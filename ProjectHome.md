Little command-line tool for generating a changelog from a Subversion log.

type <b><code>&lt;location&gt;\SvnClg.exe help</code></b> for more information.

This tool uses the StringTemplate Template Engine, so the output can easily be customized. The used template is located in the Templates folder (Templates\ChangeLog.st).<br>
For more information about StringTemplate and how to use it please visit <a href='http://www.stringtemplate.org'>http://www.stringtemplate.org</a>

<h1>Version 0.2.1</h1>

<b>General Usage:</b>

<b>Note:</b> SVN log entries that should be ignored when generating the ChangeLog can be marked by a preceding special character. By default this is a minus ('-'). It can be changed in the .config file (key="escapeChar"). This way you can easily sort out the entries you don't want to have in the ChangeLog when committing changes.<br>
<br>
<u>Available commands:</u><br>

<table><thead><th> <b>Command</b> </th><th> <b>Description</b> </th><th> <b>Usage</b> </th></thead><tbody>
<tr><td> <code>repo</code> </td><td> Specifies the repository from which the SVN-Log shall be fetched </td><td> <code>repo=svn://path/to/repo</code> </td></tr>
<tr><td> <code>output</code> or <code>o</code> </td><td> This is where the generated ChangeLog will be saved </td><td> <code>output=D:\ChangeLog.txt</code> </td></tr>
<tr><td> <code>svnpath</code> </td><td> Path to the svn.exe. This is not needed if svn.exe is already registered within the OS. </td><td> <code>svnpath=C:\path\to\svn.exe</code> </td></tr>
<tr><td> <code>range</code> or <code>r</code> </td><td> A range of revisions that shall be merged to one log entry </td><td> <code>range=x-y</code> or <code>range=all</code> or <code>range=...</code> -> <a href='Range.md'>Wiki</a> </td></tr>
<tr><td> <code>version</code> or <code>v</code> </td><td> The Version of the preceding range </td><td> <code>version=1.0.0</code> or <code>version=1.0.*</code> or <code>version=1.0.5*</code>  -> <a href='Version.md'>Wiki</a>  </td></tr>
<tr><td> <code>swap</code> </td><td> Allows to swap the order of the entries </td><td> <code>swap=true</code> or <code>swap=false</code> </td></tr>
<tr><td> <code>innerswap</code> or <code>iswap</code> </td><td> Allows to swap the order of the log messages within a changelog entry </td><td> <code>iswap=true</code> or <code>iswap=false</code> </td></tr>
<tr><td> <code>help</code> </td><td> Shows the help     </td><td> <code>help</code> </td></tr>
<tr><td> <code>textwidth</code> or <code>tw</code> </td><td> Specifies the text width. A value of -1 means the text will not be wrapped </td><td> <code>tw=70</code> </td></tr>
<tr><td> <code>escapeChar</code> or <code>esc</code> </td><td> Specifies the character that's used to ignore particular SVN log entries </td><td> <code>esc=&lt;char&gt;</code> e.g. <code>esc=-</code> </td></tr></tbody></table>

A full command line string could look something like this:<br>
<code>&lt;location&gt;\SvnClg.exe repo=http://svnclg.googlecode.com/svn/trunk/ output=D:\ChangeLog.txt range=1-100 version=1.0.0 range=101-200 version=2.0.0 swap</code>

with v0.2 it can be distilled to:<br>
<code>&lt;location&gt;\SvnClg.exe repo=http://svnclg.googlecode.com/svn/trunk/ o=D:\ChangeLog.txt r=1-100,101-200 v=1.0.0,2.0.0 swap</code>

When using none of the optional commands a ChangeLog entry will be generated for each (valid) svn log entry.<br>
<br>
<b>Note:</b> The "version" command only works with a preceding "range" command<br>
<br>
<h2>Config file (SvnClg.exe.config)</h2>
Most of the above commands can be specified as default values in the .config file.<br>
It stores default values for:<br>
- Path to svn.exe (svnPath)<br>
- The repository path (repoPath)<br>
- The output path (outputPath)<br>
- The escape character for ignoring log entries (escapeChar; default: -)<br>
- The textwidth (textWidth; default: 70)<br>
- Swap (swap; default: true)<br>
- Innerswap (innerSwap; default: true)<br>

These default settings will be overwritten when the accordant command line arguments are used.<br>
<br>
<h2>StringTemplate variables</h2>
<table><thead><th> <b>Name</b> </th><th> <b>Description</b> </th></thead><tbody>
<tr><td> $date$      </td><td> The date of the last revision in the given range </td></tr>
<tr><td> $version$   </td><td> The specified version </td></tr>
<tr><td> $authors$   </td><td> All authors that made commits </td></tr>
<tr><td> $msg$       </td><td> The log message(s) </td></tr>
<tr><td> $revisions$ </td><td> The revision numbers </td></tr>
<tr><td> $paths$     </td><td> The paths of the files that have been changed </td></tr>