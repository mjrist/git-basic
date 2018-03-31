@echo off
:: Pipes a diff against develop to the clipboard.
:: The diff can then be pasted into the UnitTestDoc for testing.
git diff origin/develop | clip
echo Unit test diff copied to clipboard.