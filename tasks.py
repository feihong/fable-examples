import signal
import subprocess
from invoke import task


@task
def serve(ctx):
    proc = subprocess.Popen('fable --watch', shell=True)
    subprocess.call('python3 -m http.server', shell=True)
    proc.send_signal(signal.SIGINT)
    proc.wait()
    print('Done!')
