# scripts

Three small harnesses for checking a change to the browser app. They exist because the two questions
that matter about an interface change — *did the words move?* and *does anything overflow?* — cannot be
answered by reading a diff, and both were answered wrongly at least once by trying.

Nothing here is part of the build. They need Node 24 or newer (for the built-in `WebSocket`) and Python 3.

## Start a Chrome that will talk to them

```bash
/Applications/Google\ Chrome.app/Contents/MacOS/Google\ Chrome \
  --remote-debugging-port=9222 --user-data-dir=/tmp/chrome-audit \
  --headless=new --no-first-run --disable-gpu about:blank &
```

On Linux, `google-chrome` or `chromium` with the same flags. The `--user-data-dir` matters: without it
Chrome may attach to your everyday profile and refuse the debugging port.

One caution that cost an afternoon: **headless Chrome clamps `--window-size` to about 500 CSS pixels
wide and crops the screenshot silently.** That is why these scripts set the viewport through
`Emulation.setDeviceMetricsOverride` instead, and why a "fix" for a phone-width layout problem should
never be trusted until the measured viewport width is printed.

## `serve.py` — serve a published build like a static host

```bash
python3 scripts/serve.py /tmp/after/wwwroot 8082
```

Threaded, because the WebAssembly runtime pulls dozens of assemblies at once and the stock
single-threaded handler serialises them into a very long wait. Unknown paths fall back to `index.html`,
which is what the Pages `404.html` copy does for deep links. It also sets `application/wasm`, without
which the runtime refuses to start.

## `rendered-text.mjs` — what the page actually shows, as text

```bash
node scripts/rendered-text.mjs http://127.0.0.1:8081/ before.txt
node scripts/rendered-text.mjs http://127.0.0.1:8082/ after.txt      # your branch
diff before.txt after.txt
```

Clears `localStorage` and reloads before capturing, because the app remembers the last setup and the
last language — two builds can only be compared from the same starting state. Takes an optional language
tag as a third argument. Prints a warning if a translation key reached the
screen, which is what happens when a resource file is missing one.

Use it to prove that a refactor changed where words come from and not what they say. Every string that
moved behind a translation key in this repository was checked this way, and the check caught a key
rendering as itself, and Razor swallowing the space between two sentences.

## `layout-audit.mjs` — every label at every width in every language

```bash
node scripts/layout-audit.mjs http://127.0.0.1:8082/
```

Walks ten widths from 360 to 1440 in all eight languages and reports only failures: text wider than its
own box, and any page that scrolls sideways. Silence is the pass condition.

**Read its silence carefully.** It once reported "no overflow anywhere" while measuring English eight
times, because switching the language did not redraw the page. If you are auditing across languages,
confirm the pages differ in length first — `rendered-text.mjs` per language will tell you in one line
each. A harness that cannot fail is not a harness.
