# DebugDispClient
Interfacing problems with signalR clients.

The DebugOmgDispClient program module is a signalR client console program. 
DebugOmgDispClient is essentially an intermediary between the desktop Qt application (client) and the system server.
Another possible reliable way of ensuring information exchange between the signalR-server and the Qt-client is proposed, 
the need for which is dictated by the still unstable existing announced information exchange methods.

DebugOmgDispClient works in multi-threaded mode, written in the C # programming language. 
It uses Microsoft ASP .NET Core version 5.0 to perform the functionality. 
The program functions in the line of Microsoft Windows (x64) operating systems, as well as in LINUX.
This project is more of an informative and familiarization mission, 
and is intended to demonstrate the ability to do without the announced C ++ - signalR implementation. 
Later, I will show what code can be on the server and on the Qt client. 
However, I will still provide some sketches of the code from the Qt client side in this project (see the documentation in the doc folder).

This simple example is more of an informative and introductory mission, 
and is intended to demonstrate the ability to do without the announced C ++ / signalR implementation. 
It uses the NET Core signalR client and named pipes instead. This allows for reliable information exchange.
