# Introduction #

Usage of the `range` command


# Details #
To specify a range of revisions the following parameters can be used:<br>
- <code>r=x-y</code> Where x is the start revision number an y the end revision number. E.g. <code>`r=1-50</code><br>
- <code>r=all</code> or <code>r=...</code> All revisions/log entries will be merged to one ChangeLog entry<br>
<br>
Multiple ranges can be sparated by commas:<br>
E.g. <code>r=1-50,51-100,...</code> This will generate 3 ChangeLog entries. The first for the revisions 1-50, the second for the revisions 51-100 and the third for all remaining revisions