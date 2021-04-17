# Umamusume msgpack to json converter
 Converts the decrypted game umamusume server communication msgpack packet to json and deserializes the included race_scianrio data to json.   
 See [here](https://github.com/CNA-Bld/Riru-CarrotJuicer) to how to get the decrypted msgpack   
 Both files ending in Q.msgpack and R.msgpack are supported.
 
## Usage 
 ```umamusume_msgpack_json_converter.exe [decryped_packet.msgpack]```   
 It will automatically convert msgpack to json and if there is race_scenario inside it will try to convert it to json too.
 
## Credit/Source
 * race_data_parser.exe : Compiled the python script in [Here](https://github.com/SSHZ-ORG/hakuraku/blob/master/umdb/) with pyinstaller.
 
