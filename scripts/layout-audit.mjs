// Measures the layout in every language at every width that matters, and reports only failures: text
// wider than the box it sits in, and any page that scrolls sideways. Silence is the pass condition.
//
// Usage:  node scripts/layout-audit.mjs <url>
//
// Read its silence carefully. This harness once reported "no overflow anywhere" while measuring English
// eight times, because changing the language redrew the header and not the page. Confirm the languages
// differ — scripts/rendered-text.mjs prints a character count per language — before believing a pass.
//
// Needs a Chrome listening for the DevTools protocol; see scripts/README.md.
if (!process.argv[2]) {
  console.error('usage: node scripts/layout-audit.mjs <url>');
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
let id = 0; const p = new Map();
ws.onmessage = e => { const m = JSON.parse(e.data); if (p.has(m.id)) { p.get(m.id)(m.result); p.delete(m.id); } };
const send = (m, params = {}) => new Promise(res => { const n = ++id; p.set(n, res); ws.send(JSON.stringify({ id: n, method: m, params })); });
const ev = async x => {
  const r = await send('Runtime.evaluate', { expression: x, returnByValue: true, awaitPromise: true });
  if (r?.exceptionDetails) throw new Error(JSON.stringify(r.exceptionDetails).slice(0, 300));
  return r.result?.value;
};
const wait = ms => new Promise(r => setTimeout(r, ms));

const LANGS = ['en', 'ru', 'uk', 'nl', 'de', 'es', 'pl', 'tr'];
const WIDTHS = [360, 390, 414, 480, 620, 768, 900, 1024, 1280, 1440];

// A label overflows when its text is wider than its own box. scrollWidth beats clientWidth by more
// than a rounding error only when something is actually clipped or pushing out.
const measure = `(() => {
  const bad = [];
  const seen = new Set();
  const kinds = ['.metric .label', '.metric .value', 'th', '.field label', '.segmented button',
                 '.chip', '.group-divider', 'h2', '.button-row button', '.button-row a', 'option'];
  for (const sel of kinds) {
    for (const el of document.querySelectorAll(sel)) {
      if (el.scrollWidth > el.clientWidth + 1 && el.clientWidth > 0) {
        const text = el.textContent.replace(/\\s+/g, ' ').trim().slice(0, 40);
        const key = sel + '|' + text;
        if (!seen.has(key)) { seen.add(key); bad.push(sel + ' :: "' + text + '" ' + el.scrollWidth + '>' + el.clientWidth); }
      }
    }
  }
  const wide = document.documentElement.scrollWidth > window.innerWidth + 1
    ? 'PAGE SCROLLS SIDEWAYS ' + document.documentElement.scrollWidth + '>' + window.innerWidth : null;
  return JSON.stringify({ bad, wide });
})()`;

await send('Page.navigate', { url: process.argv[2] });
for (let i = 0; i < 90; i++) { if (await ev(`document.querySelectorAll('.metrics').length > 0`)) break; await wait(500); }
await wait(1500);

let failures = 0;
for (const lang of LANGS) {
  const set = await ev(`(() => {
    const s = document.querySelector('select.language');
    if (!s) return 'no picker';
    s.value = '${lang}';
    s.dispatchEvent(new Event('change', { bubbles: true }));
    return true;
  })()`);
  if (set !== true) { console.log(lang, '->', set); continue; }
  await wait(900);

  for (const w of WIDTHS) {
    await send('Emulation.setDeviceMetricsOverride', { width: w, height: 1400, deviceScaleFactor: 1, mobile: w < 620 });
    await wait(450);
    const { bad, wide } = JSON.parse(await ev(measure));
    if (wide || bad.length) {
      failures++;
      console.log(`\n${lang} @ ${w}px`);
      if (wide) console.log('   ' + wide);
      for (const b of bad.slice(0, 8)) console.log('   ' + b);
      if (bad.length > 8) console.log(`   … and ${bad.length - 8} more`);
    }
  }
  process.stdout.write(`${lang} done  `);
}
console.log(`\n\n${failures ? failures + ' width/language combinations with overflow' : 'no overflow anywhere'}`);
ws.close();
