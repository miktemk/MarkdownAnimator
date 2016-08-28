#!/bin/sh
# NOTE: when writing this script with Sublime make sure the line endings are Unix!!!

git update-index --no-assume-unchanged \$bin/MarkdownAnimator.exe
git update-index --no-assume-unchanged \$bin/Miktemk.TextToSpeech.dll
git update-index --no-assume-unchanged \$bin/Miktemk.Winforms.dll
git update-index --no-assume-unchanged \$bin/Miktemk.dll
git update-index --no-assume-unchanged \$bin/MarkdownUtils.dll
