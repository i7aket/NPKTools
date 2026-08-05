"""Serve the published Blazor output the way a static host does.

Threaded because the runtime pulls dozens of assemblies at once and the stock
single-threaded handler serialises them into a very long wait. Every unknown path
falls back to index.html, which is what the Pages 404.html copy does for deep links.
"""
import mimetypes
import os
import sys
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer

ROOT = sys.argv[1]
PORT = int(sys.argv[2])

for extension, kind in {
    ".wasm": "application/wasm",
    ".js": "text/javascript",
    ".json": "application/json",
    ".dat": "application/octet-stream",
    ".blat": "application/octet-stream",
    ".pdb": "application/octet-stream",
}.items():
    mimetypes.add_type(kind, extension)


class Handler(SimpleHTTPRequestHandler):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, directory=ROOT, **kwargs)

    def end_headers(self):
        # No caching, so a rebuild is visible on refresh rather than a stale mix of old and new.
        self.send_header("Cache-Control", "no-store")
        super().end_headers()

    def send_head(self):
        path = self.translate_path(self.path)
        if not os.path.exists(path) and "." not in os.path.basename(path):
            self.path = "/index.html"
        return super().send_head()

    def log_message(self, fmt, *args):
        # Everything, not just failures: when a boot stalls, the useful fact is which file was the
        # last one fetched, and that is invisible if the successful requests are filtered out.
        agent = self.headers.get("User-Agent", "")
        who = "ipad" if "Mobile" in agent or "iPhone" in agent or "iPad" in agent else "other"
        sys.stderr.write(f"[{who}] {fmt % args}\n")
        sys.stderr.flush()


ThreadingHTTPServer(("0.0.0.0", PORT), Handler).serve_forever()
