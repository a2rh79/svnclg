# Introduction #

Usage of the `version` command


# Details #

Note: The "version" command only works with a preceding "range" command.<br>
E.g. <code>r=1-50 v=1.0.0</code>

The versions can be separated by commas same as the ranges.<br>
E.g. <code>r=1-50,51-100 v=1.0.0,2.0.0</code>

Theres also the option to specify version number with a <code>*</code>.<br>
E.g. r=1-50,51-100 v=1.1.<code>*</code> The <code>*</code> will then be automatically replaced by a 0 and will<br>
be incremented by 1 with each given range, i.e. 1.1.0, 1.1.1, ...<br>
or r=1-50,51-100 v=1.1.5<code>*</code> The version number will be incremented by 1, starting with<br>
the given number (in this case 5), i.e. 1.1.5, 1.1.6, ...<br>
<br>
Another example:<br>
r=1-50,51-100,101-180 v=1.0.0,2.0.<code>*</code> will generate 3 entries for the 3 given ranges with the versions 1.0.0(r=1-50), 2.0.0(r=51-100), 2.0.1(r=101-180)