import subprocess
from invoke import task


@task
def serve(ctx):
    subprocess.call('python3 -m http.server', shell=True)
