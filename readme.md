This is just a fun project to help me shake off my C# rust.

The idea though is to limit the amount of object instantiation as much as possible, in order to reduce mememory fragmentation, locking, and GC activity.

Basically, a fixed number of "Redis" objects are created. Each redis object has a fixed buffer and a connection. When you are writing or reading, it'll try to use the fix buffer if it's large enough, else it'llcreate one. The goal is to hav 90% of all commands fit within the buffer. As for connections, they can time out or mess up, so the Redis object can always kill its connection and get a new one.

