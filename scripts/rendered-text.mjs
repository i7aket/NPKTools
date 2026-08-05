// Captures what a page actually shows, as text, so two builds can be diffed.
//
// Usage:  node scripts/rendered-text.mjs <url> <outfile> [language-tag]
//
// Why this exists: "nothing visible changed" is a claim, and reading a diff does not check it. Razor
// drops literal whitespace between an expression and the block that follows, so a change whose diff
// looks obviously correct can run two sentences together. Capturing the rendered text and diffing that
// catches it; nothing else did.
//
// Why innerText and not textContent or a text-node walk: only innerText reflects what the layout shows.
// Walking text nodes and trimming each one invents a space wherever two nodes touch. textContent keeps
// the insignificant whitespace that a wrapped line of markup leaves inside a block element. Both report
// differences that are not on the screen — and both did, before that was understood.
//
// Needs a Chrome listening for the DevTools protocol; see scripts/README.md.

const target = process.argv[2];
const outfile = process.argv[3];
const language = process.argv[4];

if (!target || !outfile) {
  console.error('usage: node scripts/rendered-text.mjs <url> <outfile> [language-tag]');
  process.exit(2);
}

const tabs = await (await fetch('http://127.0.0.1:9222/json')).json();
const page = tabs.find(x => x.type === 'page');
if (!page) {
  console.error('no page tab on 127.0.0.1:9222 — see scripts/README.md');
  process.exit(2);
}

const ws = new WebSocket(page.webSocketDebuggerUrl);
await new Promise(r => (ws.onopen = r));

let id = 0;
const pending = new Map();
ws.onmessage = e => {
  const m = JSON.parse(e.data);
  if (pending.has(m.id)) { pending.get(m.id)(m.result); pending.delete(m.id); }
};
const send = (method, params = {}) =>
  new Promise(res => { const n = ++id; pending.set(n, res); ws.send(JSON.stringify({ id: n, method, params })); });
const ev = async expression => {
  const r = await send('Runtime.evaluate', { expression, returnByValue: true, awaitPromise: true });
  if (r?.exceptionDetails) throw new Error(JSON.stringify(r.exceptionDetails).slice(0, 300));
  return r.result?.value;
};
const wait = ms => new Promise(r => setTimeout(r, ms));

// Wide enough that nothing is in its mobile layout, tall enough that everything is laid out at once.
await send('Emulation.setDeviceMetricsOverride', { width: 1600, height: 2400, deviceScaleFactor: 1, mobile: false });

// The app is WebAssembly: the first paint is a loading message, and the runtime takes a few seconds
// cold. Wait for something only the loaded app renders rather than for a fixed delay.
const load = async () => {
  await send('Page.navigate', { url: target });
  for (let i = 0; i < 120; i++) {
    if (await ev(`document.querySelectorAll('.metrics').length > 0`)) break;
    await wait(500);
  }
  if (!(await ev(`document.querySelectorAll('.metrics').length > 0`))) {
    console.error('NOT LOADED — the app never finished starting, so the capture would be meaningless');
    process.exit(1);
  }
  await wait(1200);
};

await load();

// Cleared and reloaded, because the app remembers the last setup and the last language in
// localStorage. Two builds can only be compared from the same starting state, and a capture that
// silently inherited the previous run's language is exactly the kind of result that looks fine.
await ev(`localStorage.clear()`);
await load();

if (language) {
  const set = await ev(`(() => {
    const s = document.querySelector('select.language');
    if (!s) return 'no language picker';
    s.value = ${JSON.stringify(language)};
    s.dispatchEvent(new Event('change', { bubbles: true }));
    return true;
  })()`);
  if (set !== true) { console.error(set); process.exit(1); }
  await wait(1000);
}

// Collapsed sections hold real copy, so open them before reading.
await ev(`document.querySelectorAll('details').forEach(d => (d.open = true)); true`);
await wait(700);

const text = await ev(`document.body.innerText
  .split(/[\\r\\n]+/)
  .map(s => s.replace(/\\s+/g, ' ').trim())
  .filter(Boolean)
  .join('\\n')`);

const { writeFileSync } = await import('node:fs');
writeFileSync(outfile, (text ?? '') + '\n');

// A key that reached the screen means a resource file is missing it — the fallback shows the key
// itself, which is ugly on purpose so it gets reported. Worth flagging here rather than in a diff.
const leaked = [...new Set((text ?? '').match(/\b[a-z]+(?:\.[a-zA-Z]+){1,3}\b/g) ?? [])]
  .filter(s => !/\.(json|js|css|com|org|io|md|png)$/.test(s));

console.log(`captured ${text?.length ?? 0} characters to ${outfile}`);
if (leaked.length) console.log(`LEAKED KEYS: ${leaked.slice(0, 10).join(', ')}`);

ws.close();
