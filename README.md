Welcome to SimpleLanguage!

CLI INSTRUCTIONS:<br>
Use $ to write a command<br>
Available commands:<br>
  Global commands:<br>
&emsp;&emsp;$project add <i>project_name</i> <i>project_directory(use & at the start to mark the path as relative to the SimpleLanguage.exe file)</i> : adds a project<br> 
&emsp;&emsp;$project delete <i>project_name</i> : deletes a project<br>
<br>
&emsp;Interpreter commands:<br>
&emsp;&emsp;$print stack : prints the last stack<br>

<b>PROGRAMMING:</b><br>
&emsp;Instructions:<br>
&emsp;&emsp;Stack managment:<br>
&emsp;&emsp;&emsp;PUSH - pushes the specified values to the top of the main stack<br>
&emsp;&emsp;&emsp;POP - pops the top most value from the main stack<br>
&emsp;&emsp;&emsp;PUT - moves the top most element of the main stack to the storage stack<br>
&emsp;&emsp;&emsp;GET - moves the top most element of the storage stack to the main stack<br>
&emsp;&emsp;&emsp;COPY - duplicates the top most element of the main stack<br>
&emsp;&emsp;&emsp;SWAP - swaps the main stack and the storage stack<br>
&emsp;&emsp;&emsp;CLEARMAIN, CLEARSTOR - clears the designated stacks<br>
<br>
&emsp;&emsp;Input-Output:<br>
&emsp;&emsp;&emsp;PRINT - prints the top most element of the main stack to the console<br>
&emsp;&emsp;&emsp;PRINTC - prints the top most element of the main stack as a character (using UNICODE encoding) to the console<br>
