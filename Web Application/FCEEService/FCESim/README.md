# FCEE Mock Testing Tool

Sends mock FCEE messages to FuelsConnect.

## Usage

To use this script, simply create a data.json file and place it in the folder alongside the compiled binary, input the information about the proper IMEI, Message Type, and Interval between messages information as shown below:
{
    "APIURL": "https://fcee.az.fueldepot-gov.net/api/v1/",
    "IMEI": "999999999999999",
    "MsgType": 13,
    "IntervalSeconds": 5
}

Then, to run the script, change directory into the proper directory with the script then run:
        dotnet build
        dotnet run
and let it run as long as necessary. When enough messages have been send, you may hit the q key to end the automation.