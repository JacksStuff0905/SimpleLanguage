<h1><b>Welcome to SimpleLanguage!</b></h1>

<b>CLI INSTRUCTIONS:</b><br>
Use $ to write a command<br>
<b>Available commands:</b><br>
&emsp;<b>Global commands:</b><br>
&emsp;&emsp;$project add <i>project_name</i> <i>project_directory(use & at the start to mark the path as relative to the SimpleLanguage.exe file)</i> : adds a project<br> 
&emsp;&emsp;$project delete <i>project_name</i> : deletes a project<br>
&emsp;&emsp;$quit : quits the current app<br>
&emsp;&emsp;$app info : prints the current app info such as its full name<br>
<br>
&emsp;<b>Interpreter commands:</b><br>
&emsp;&emsp;$print stack : prints the last stack<br>

<b>PROGRAMMING:</b><br>
&emsp;<b>Instructions:</b><br>
&emsp;&emsp;<b>Stack managment:</b><br>
&emsp;&emsp;&emsp;PUSH - pushes the specified values to the top of the main stack<br>
&emsp;&emsp;&emsp;POP - pops the top most value from the main stack<br>
&emsp;&emsp;&emsp;PUT - moves the top most element of the main stack to the storage stack<br>
&emsp;&emsp;&emsp;GET - moves the top most element of the storage stack to the main stack<br>
&emsp;&emsp;&emsp;COPY - duplicates the top most element of the main stack<br>
&emsp;&emsp;&emsp;SWAP - swaps the main stack and the storage stack<br>
&emsp;&emsp;&emsp;CLEARMAIN, CLEARSTOR - clears the designated stacks<br>
<br>
&emsp;&emsp;<b>Input-Output:</b><br>
&emsp;&emsp;&emsp;PRINT - prints the top most element of the main stack to the console<br>
&emsp;&emsp;&emsp;PRINTC - prints the top most element of the main stack as a character (using UNICODE encoding) to the console<br>
&emsp;&emsp;&emsp;INPUT - asks the user for input and pushes it onto the main stack<br>
<br>
&emsp;&emsp;<b>Math:</b><br>
&emsp;&emsp;&emsp;ADD - adds the two top most elements of the main stack and pushes the result<br>
&emsp;&emsp;&emsp;SUB - subtracts the two top most elements of the main stack and pushes the result<br>
&emsp;&emsp;&emsp;MUL - multiplies the two top most elements of the main stack and pushes the result<br>
&emsp;&emsp;&emsp;DIV - divides the two top most elements of the main stack and pushes the result<br>
<br>
&emsp;&emsp;<b>Flow control:</b><br>
&emsp;&emsp;&emsp;HALT - halts the program<br>
&emsp;&emsp;&emsp;JMP - jumps to the selected line<br>
&emsp;&emsp;&emsp;JMPEQ0 - jumps to the selected line if the value on top of the main stack is equal 0<br>
&emsp;&emsp;&emsp;JMPGT0 - jumps to the selected line if the value on top of the main stack is greater than 0<br>
&emsp;&emsp;&emsp;JMPMAIN, JMPSTOR - jumps to the selected line if the designated stack is not empty<br>
&emsp;&emsp;&emsp;JMPNOMAIN, JMPNOSTOR - jumps to the selected line if the designated stack is empty<br>
