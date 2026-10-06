#!/usr/bin/env python3
"""校验生成页、内部链接、图片、锚点，以及百科依赖源码的新鲜度。"""
import argparse,hashlib,json,re,sys
from pathlib import Path
from html.parser import HTMLParser
from urllib.parse import urlsplit,unquote
ROOT=Path(__file__).resolve().parents[1]
parser=argparse.ArgumentParser(description=__doc__)
parser.add_argument('--site-only',action='store_true',help='Only validate the published snapshot; do not claim source freshness.')
parser.add_argument('--site',type=Path,default=ROOT/'wiki/site',help='Static output directory to validate.')
args=parser.parse_args()
SITE=args.site.resolve()
errors=[]
class Page(HTMLParser):
    def __init__(self,text):
        super().__init__();self.refs=[];self.ids=set();self.feed(text)
    def handle_starttag(self,tag,attrs):
        attrs=dict(attrs)
        if 'id' in attrs:self.ids.add(attrs['id'])
        for k in ['href','src']:
            if k in attrs:self.refs.append(attrs[k])
pages={p:Page(p.read_text()) for p in SITE.rglob('*.html')}
if SITE/'index.html' not in pages:errors.append('index.html: homepage missing')
def check_ref(p,ref):
    url=urlsplit(ref)
    if url.scheme or url.netloc:
        if url.scheme=='file':errors.append(f'{p.name}: local file URL {ref}')
        return
    if unquote(url.path).startswith('/'):
        errors.append(f'{p.name}: root-relative URL breaks project Pages path: {ref}')
        return
    target=(p.parent/unquote(url.path)).resolve() if url.path else p
    if not target.is_relative_to(SITE):errors.append(f'{p.name}: outside published directory: {ref}')
    elif not target.exists():errors.append(f'{p.name}: missing {ref}')
    elif url.fragment and target in pages and unquote(url.fragment) not in pages[target].ids:errors.append(f'{p.name}: missing anchor {ref}')
for p,parsed in pages.items():
    for ref in parsed.refs:
        check_ref(p,ref)
for p in SITE.rglob('*.css'):
    for ref in re.findall(r'url\(\s*[\'\"]?([^\'\"\)]+)[\'\"]?\s*\)',p.read_text()):check_ref(p,ref.strip())
changed=[]
if not args.site_only:
    manifest=json.loads((SITE/'source-manifest.json').read_text())
    for path,expected in manifest['files'].items():
        file=ROOT/path
        if not file.exists() or hashlib.sha256(file.read_bytes()).hexdigest()!=expected:changed.append(path)
for error in errors:print('ERROR',error)
for path in changed:print('STALE',path)
freshness='source freshness: NOT CHECKED (published snapshot)' if args.site_only else f'{len(changed)} changed source files'
print(f'{len(pages)} pages, links / assets / anchors / project paths: {"PASS" if not errors else "FAIL"}; {freshness}')
sys.exit(1 if errors or changed else 0)
