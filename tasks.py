import signal
import subprocess
import contextlib
from pathlib import Path
from invoke import task
from flask import Flask, request, send_from_directory


@task
def serve(ctx):
    with fable_watch():
        app.run(port=8000)


app = Flask(__name__)
cwd = Path.cwd()

@app.route('/', defaults={'path': ''})
@app.route('/<path:path>')
def catch_all(path):
    filepath = cwd / path
    if filepath.is_dir():
        index_path = filepath / 'index.html'
        if index_path.exists():
            return index_path.read_text()
    if filepath.exists():
        return send_from_directory(str(cwd), path)
    return 'not found', 404

@app.route('/upper/')
def upper():
    return request.args.get('text', '').upper()


@contextlib.contextmanager
def fable_watch():
    proc = subprocess.Popen('fable --watch', shell=True)
    yield
    proc.send_signal(signal.SIGINT)
    proc.wait()
    print('Done!')
