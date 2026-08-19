DECLARE @PointTemplateGuid UNIQUEIDENTIFIER = '0ADB4947-1CC4-4A44-91F8-E76F281EA718'
DECLARE @ProfileImageGuid UNIQUEIDENTIFIER = (SELECT PictureGuid FROM dbo.tblPictures WHERE Id = 'Tank Template')
DECLARE @TankSiteGuid UNIQUEIDENTIFIER  = '00000000-0000-0000-0000-000000000001'

-- Strap Table is the first Standard Module Developed, for Single Site System Modules are ownership changed to single site
IF EXISTS (SELECT SiteGuid FROM tblModule WHERE ModuleGuid = @StrapTableModuleGuid)
BEGIN
	SET @TankSiteGuid =  (SELECT SiteGuid FROM tblModule WHERE ModuleGuid = @StrapTableModuleGuid);
END 



DECLARE @DefaultDrawingGuid UNIQUEIDENTIFIER = NULL

-- updates to this will require additional work to merge with existing standard template to not overwrite custom commands
DECLARE @StandardTankPointCommandStatus XML =
'<PointCommandStatus xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <CommandStatusLists>
	<PointCommandStatusList>
		<CommandStatusListGuid>b0738ddf-d778-478e-8015-f93462ad4533</CommandStatusListGuid>
		<ID>Enraf Model 854</ID>
		<CommandStatusList>
		<CommandStatusElement>
			<Key>Follow Level</Key>
			<Value>276</Value>
		</CommandStatusElement>
		<CommandStatusElement>
			<Key>Unlock</Key>
			<Value>169</Value>
		</CommandStatusElement>
		<CommandStatusElement>
			<Key>Test</Key>
			<Value>226</Value>
		</CommandStatusElement>
		<CommandStatusElement>
			<Key>Lock Test</Key>
			<Value>260</Value>
		</CommandStatusElement>
		<CommandStatusElement>
			<Key>Block</Key>
			<Value>258</Value>
		</CommandStatusElement>
		<CommandStatusElement>
			<Key>Upper Density</Key>
			<Value>306</Value>
		</CommandStatusElement>
		<CommandStatusElement>
			<Key>Find Water Level</Key>
			<Value>197</Value>
		</CommandStatusElement>
		</CommandStatusList>
	</PointCommandStatusList>
	<PointCommandStatusList>
		<CommandStatusListGuid>af943503-0f3d-4c22-9966-9e3272c0f224</CommandStatusListGuid>
		<ID>Varec Model 6000</ID>
		<CommandStatusList>
		<CommandStatusElement>
			<Key>Follow Level</Key>
			<Value>276</Value>
		</CommandStatusElement>
		<CommandStatusElement>
			<Key>Raise</Key>
			<Value>129</Value>
		</CommandStatusElement>
		<CommandStatusElement>
			<Key>Stop</Key>
			<Value>149</Value>
		</CommandStatusElement>
		<CommandStatusElement>
			<Key>Find Water Level</Key>
			<Value>197</Value>
		</CommandStatusElement>
		<CommandStatusElement>
			<Key>Upper Density</Key>
			<Value>306</Value>
		</CommandStatusElement>
		<CommandStatusElement>
			<Key>Reset</Key>
			<Value>135</Value>
		</CommandStatusElement>
		<CommandStatusElement>
			<Key>Run Immediate Profile</Key>
			<Value>195</Value>
		</CommandStatusElement>
		<CommandStatusElement>
			<Key>Test</Key>
			<Value>226</Value>
		</CommandStatusElement>
		</CommandStatusList>
	</PointCommandStatusList>
  </CommandStatusLists>
</PointCommandStatus>'

-- updates to this will require additional work to merge with existing standard template to not overwrite custom alarms
DECLARE @StandardTankDeviceAlarmMaps XML = 
'<ArrayOfDeviceAlarmMap xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
	<DeviceAlarmMap>
		<DeviceAlarmMapEntryList>
		<DeviceAlarmMapEntry>
			<TestName>Scan Failure</TestName>
			<BitMask>1</BitMask>
			<AlarmPriority>402a7722-062b-42f6-b6a5-e6180e2ba2b8</AlarmPriority>
		</DeviceAlarmMapEntry>
		<DeviceAlarmMapEntry>
			<TestName>Input Output Failure</TestName>
			<BitMask>2</BitMask>
			<AlarmPriority>402a7722-062b-42f6-b6a5-e6180e2ba2b8</AlarmPriority>
		</DeviceAlarmMapEntry>
		<DeviceAlarmMapEntry>
			<TestName>Device</TestName>
			<BitMask>4</BitMask>
			<AlarmPriority>402a7722-062b-42f6-b6a5-e6180e2ba2b8</AlarmPriority>
		</DeviceAlarmMapEntry>
		<DeviceAlarmMapEntry>
			<TestName>Configuration Change</TestName>
			<BitMask>8</BitMask>
			<AlarmPriority>402a7722-062b-42f6-b6a5-e6180e2ba2b8</AlarmPriority>
		</DeviceAlarmMapEntry>
		<DeviceAlarmMapEntry>
			<TestName>Raise Failure</TestName>
			<BitMask>16</BitMask>
			<AlarmPriority>402a7722-062b-42f6-b6a5-e6180e2ba2b8</AlarmPriority>
		</DeviceAlarmMapEntry>
		<DeviceAlarmMapEntry>
			<TestName>Lower Failure</TestName>
			<BitMask>32</BitMask>
			<AlarmPriority>402a7722-062b-42f6-b6a5-e6180e2ba2b8</AlarmPriority>
		</DeviceAlarmMapEntry>
		<DeviceAlarmMapEntry>
			<TestName>Upload Failure</TestName>
			<BitMask>64</BitMask>
			<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
		</DeviceAlarmMapEntry>
		<DeviceAlarmMapEntry>
			<TestName>Download Failure</TestName>
			<BitMask>128</BitMask>
			<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
		</DeviceAlarmMapEntry>
		<DeviceAlarmMapEntry>
			<TestName>Overfill</TestName>
			<BitMask>256</BitMask>
			<AlarmPriority>aa9e557c-a652-4caf-9bca-2bcb9ab5b104</AlarmPriority>
		</DeviceAlarmMapEntry>
		</DeviceAlarmMapEntryList>
		<ID>8130 RTU</ID>
		<DeviceAlarmMapGuid>0FA75BC6-97CC-4BED-ACFD-665EA6E7E062</DeviceAlarmMapGuid>
		<NotAlarmText>Normal</NotAlarmText>
		<AlarmCategory>512ab266-b3b8-4a29-b8d9-594795cf63ed</AlarmCategory>
		<NormalUnacknowledgedPriority>5b7d7344-7d3c-4cde-a834-b5e2c8bfe11f</NormalUnacknowledgedPriority>
	</DeviceAlarmMap>
	<DeviceAlarmMap>
		<DeviceAlarmMapEntryList>
			<DeviceAlarmMapEntry>
				<TestName>Scan Failure</TestName>
				<BitMask>1</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>No Update</TestName>
				<BitMask>4</BitMask>
				<AlarmPriority>402a7722-062b-42f6-b6a5-e6180e2ba2b8</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Level Product Invalid</TestName>
				<BitMask>16</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Temperature Product Invalid</TestName>
				<BitMask>32</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
		</DeviceAlarmMapEntryList>
		<ID>8810 RTU - Tank Status</ID>
			<DeviceAlarmMapGuid>59bf2752-b783-a133-3e94-cc98fb820302</DeviceAlarmMapGuid>
			<NotAlarmText>Normal</NotAlarmText>
			<AlarmCategory>512ab266-b3b8-4a29-b8d9-594795cf63ed</AlarmCategory>
			<NormalUnacknowledgedPriority>5b7d7344-7d3c-4cde-a834-b5e2c8bfe11f</NormalUnacknowledgedPriority>
	</DeviceAlarmMap>
	<DeviceAlarmMap>
		<DeviceAlarmMapEntryList>
			<DeviceAlarmMapEntry>
				<TestName>Module Not Installed</TestName>
				<BitMask>16777216</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Module Mismatch</TestName>
				<BitMask>33554432</BitMask>
				<AlarmPriority>aa9e557c-a652-4caf-9bca-2bcb9ab5b104</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Module Hardware Error</TestName>
				<BitMask>67108864</BitMask>
				<AlarmPriority>aa9e557c-a652-4caf-9bca-2bcb9ab5b104</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Module Unknown Type</TestName>
				<BitMask>134217728</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
		</DeviceAlarmMapEntryList>
		<ID>8810 RTU - Module Status</ID>
			<DeviceAlarmMapGuid>3EE21EDF-9CFC-4b0c-BE54-0C16AC131EFD</DeviceAlarmMapGuid>
			<NotAlarmText>Normal</NotAlarmText>
			<AlarmCategory>512ab266-b3b8-4a29-b8d9-594795cf63ed</AlarmCategory>
			<NormalUnacknowledgedPriority>5b7d7344-7d3c-4cde-a834-b5e2c8bfe11f</NormalUnacknowledgedPriority>
	</DeviceAlarmMap>
	<DeviceAlarmMap>
		<DeviceAlarmMapEntryList>
			<DeviceAlarmMapEntry>
				<TestName>Channel Timeout</TestName>
				<BitMask>1</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Channel Initialization Failure</TestName>
				<BitMask>2</BitMask>
				<AlarmPriority>aa9e557c-a652-4caf-9bca-2bcb9ab5b104</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Channel Hardware Error</TestName>
				<BitMask>4</BitMask>
				<AlarmPriority>aa9e557c-a652-4caf-9bca-2bcb9ab5b104</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Serial USB Device Controller Error</TestName>
				<BitMask>8</BitMask>
				<AlarmPriority>aa9e557c-a652-4caf-9bca-2bcb9ab5b104</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Channel Transmit Data Error</TestName>
				<BitMask>16</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Channel Disabled</TestName>
				<BitMask>32</BitMask>
				<AlarmPriority>402a7722-062b-42f6-b6a5-e6180e2ba2b8</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Channel Protocol Mismatch</TestName>
				<BitMask>64</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>DIO Input Channel Override</TestName>
				<BitMask>128</BitMask>
				<AlarmPriority>402a7722-062b-42f6-b6a5-e6180e2ba2b8</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>DIO Output Channel Mismatch</TestName>
				<BitMask>256</BitMask>
				<AlarmPriority>402a7722-062b-42f6-b6a5-e6180e2ba2b8</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Mark Space Line Shorted</TestName>
				<BitMask>512</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Field Power Failure</TestName>
				<BitMask>1024</BitMask>
				<AlarmPriority>aa9e557c-a652-4caf-9bca-2bcb9ab5b104</AlarmPriority>
			</DeviceAlarmMapEntry>
		</DeviceAlarmMapEntryList>
		<ID>8810 RTU - Channel Status</ID>
			<DeviceAlarmMapGuid>39DF7445-5F1A-4d37-93C7-F4E6B7BCEAA0</DeviceAlarmMapGuid>
			<NotAlarmText>Normal</NotAlarmText>
			<AlarmCategory>512ab266-b3b8-4a29-b8d9-594795cf63ed</AlarmCategory>
			<NormalUnacknowledgedPriority>5b7d7344-7d3c-4cde-a834-b5e2c8bfe11f</NormalUnacknowledgedPriority>
	</DeviceAlarmMap>
	<DeviceAlarmMap>
		<DeviceAlarmMapEntryList>
			<DeviceAlarmMapEntry>
				<TestName>Scan Failure</TestName>
				<BitMask>1</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Timeout</TestName>
				<BitMask>2</BitMask>
				<AlarmPriority>402a7722-062b-42f6-b6a5-e6180e2ba2b8</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Level Product Invalid</TestName>
				<BitMask>4</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Temperature Product Invalid</TestName>
				<BitMask>8</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>CPU Timeout</TestName>
				<BitMask>2147483648</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
		</DeviceAlarmMapEntryList>
		<ID>EN811 EN873 EN990 - Gauge Status</ID>
			<DeviceAlarmMapGuid>6DBB95DE-0C33-4126-9C1F-C9E27FE174A6</DeviceAlarmMapGuid>
			<NotAlarmText>Normal</NotAlarmText>
			<AlarmCategory>512ab266-b3b8-4a29-b8d9-594795cf63ed</AlarmCategory>
			<NormalUnacknowledgedPriority>5b7d7344-7d3c-4cde-a834-b5e2c8bfe11f</NormalUnacknowledgedPriority>
	</DeviceAlarmMap>
		<DeviceAlarmMap>
		<DeviceAlarmMapEntryList>
			<DeviceAlarmMapEntry>
				<TestName>Scan Failure</TestName>
				<BitMask>1</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Timeout</TestName>
				<BitMask>2</BitMask>
				<AlarmPriority>402a7722-062b-42f6-b6a5-e6180e2ba2b8</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Level Product Invalid</TestName>
				<BitMask>4</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Temperature Product Invalid</TestName>
				<BitMask>8</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Density Invalid</TestName>
				<BitMask>1073741824</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>CIU Timeout</TestName>
				<BitMask>2147483648</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
		</DeviceAlarmMapEntryList>
		<ID>EN854 - Gauge Status</ID>
			<DeviceAlarmMapGuid>1A52A09A-C820-4f5c-B0CC-1AC9A8C7830E</DeviceAlarmMapGuid>
			<NotAlarmText>Normal</NotAlarmText>
			<AlarmCategory>512ab266-b3b8-4a29-b8d9-594795cf63ed</AlarmCategory>
			<NormalUnacknowledgedPriority>5b7d7344-7d3c-4cde-a834-b5e2c8bfe11f</NormalUnacknowledgedPriority>
	</DeviceAlarmMap>
	<DeviceAlarmMap>
		<DeviceAlarmMapEntryList>
			<DeviceAlarmMapEntry>
				<TestName>Scan Failure</TestName>
				<BitMask>1</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Timeout</TestName>
				<BitMask>2</BitMask>
				<AlarmPriority>402a7722-062b-42f6-b6a5-e6180e2ba2b8</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Level Product Invalid</TestName>
				<BitMask>4</BitMask>	
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Temperature Product Invalid</TestName>
				<BitMask>8</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>EE Bad</TestName>
				<BitMask>4096</BitMask>
				<AlarmPriority>aa9e557c-a652-4caf-9bca-2bcb9ab5b104</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>EE Checksum Bad</TestName>
				<BitMask>8192</BitMask>
				<AlarmPriority>aa9e557c-a652-4caf-9bca-2bcb9ab5b104</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>RAM Bad</TestName>
				<BitMask>16384</BitMask>
				<AlarmPriority>aa9e557c-a652-4caf-9bca-2bcb9ab5b104</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>EPROM Bad</TestName>
				<BitMask>32768</BitMask>
				<AlarmPriority>aa9e557c-a652-4caf-9bca-2bcb9ab5b104</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Local Modifications</TestName>
				<BitMask>65536</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Calculation Bad</TestName>
				<BitMask>131072</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Error Configuration</TestName>
				<BitMask>262144</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Error Calculation</TestName>
				<BitMask>524288</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Bad CPU Board</TestName>
				<BitMask>1048576</BitMask>
				<AlarmPriority>aa9e557c-a652-4caf-9bca-2bcb9ab5b104</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Bad Comms Board</TestName>
				<BitMask>2097152</BitMask>
				<AlarmPriority>aa9e557c-a652-4caf-9bca-2bcb9ab5b104</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Level Stale</TestName>
				<BitMask>4194304</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Response 40 Bit</TestName>
				<BitMask>8388608</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Response Low Speed</TestName>
				<BitMask>16777216</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Power VIn</TestName>
				<BitMask>33554432</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Power Vf</TestName>
				<BitMask>67108864</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Short Space Line</TestName>
				<BitMask>134217728</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Short Mark Line</TestName>
				<BitMask>268435456</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Fuse Blown</TestName>
				<BitMask>536870912</BitMask>
				<AlarmPriority>aa9e557c-a652-4caf-9bca-2bcb9ab5b104</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Power Fail</TestName>
				<BitMask>1073741824</BitMask>
				<AlarmPriority>aa9e557c-a652-4caf-9bca-2bcb9ab5b104</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Encoder Battery Low</TestName>
				<BitMask>2147483648</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
		</DeviceAlarmMapEntryList>
		<ID>ATT4000 - Gauge Status</ID>
			<DeviceAlarmMapGuid>EB2BB6D4-4817-4912-9165-C5F67FB71E4E</DeviceAlarmMapGuid>
			<NotAlarmText>Normal</NotAlarmText>
			<AlarmCategory>512ab266-b3b8-4a29-b8d9-594795cf63ed</AlarmCategory>
			<NormalUnacknowledgedPriority>5b7d7344-7d3c-4cde-a834-b5e2c8bfe11f</NormalUnacknowledgedPriority>
	</DeviceAlarmMap>

	<DeviceAlarmMap>
		<DeviceAlarmMapEntryList>
			<DeviceAlarmMapEntry>
				<TestName>Scan Failure</TestName>
				<BitMask>1</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Timeout</TestName>
				<BitMask>2</BitMask>
				<AlarmPriority>402a7722-062b-42f6-b6a5-e6180e2ba2b8</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Level Product Invalid</TestName>
				<BitMask>4</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Temperature Product Invalid</TestName>
				<BitMask>8</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>

			<DeviceAlarmMapEntry>
				<TestName>Level Stale</TestName>
				<BitMask>4194304</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Response 40 Bit</TestName>
				<BitMask>8388608</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Response Low Speed</TestName>
				<BitMask>16777216</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Power VIn</TestName>
				<BitMask>33554432</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Power Vf</TestName>
				<BitMask>67108864</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Short Space Line</TestName>
				<BitMask>134217728</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Short Mark Line</TestName>
				<BitMask>268435456</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Fuse Blown</TestName>
				<BitMask>536870912</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Power Fail</TestName>
				<BitMask>1073741824</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>CiU Timeout</TestName>
				<BitMask>2147483648</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
		</DeviceAlarmMapEntryList>
		<ID>FTT29xx - Gauge Status</ID>
			<DeviceAlarmMapGuid>E8479EA6-1829-4648-B839-540577B2C88A</DeviceAlarmMapGuid>
			<NotAlarmText>Normal</NotAlarmText>
			<AlarmCategory>512ab266-b3b8-4a29-b8d9-594795cf63ed</AlarmCategory>
			<NormalUnacknowledgedPriority>5b7d7344-7d3c-4cde-a834-b5e2c8bfe11f</NormalUnacknowledgedPriority>
	</DeviceAlarmMap>

	<DeviceAlarmMap>
		<DeviceAlarmMapEntryList>
			<DeviceAlarmMapEntry>
				<TestName>Scan Failure</TestName>
				<BitMask>1</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Timeout</TestName>
				<BitMask>2</BitMask>
				<AlarmPriority>402a7722-062b-42f6-b6a5-e6180e2ba2b8</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Level Product Invalid</TestName>
				<BitMask>4</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Temperature Product Invalid</TestName>
				<BitMask>8</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Level Stale</TestName>
				<BitMask>4194304</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Response 40 Bit</TestName>
				<BitMask>8388608</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Response Low Speed</TestName>
				<BitMask>16777216</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Power VIn</TestName>
				<BitMask>33554432</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Power Vf</TestName>
				<BitMask>67108864</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Short Space Line</TestName>
				<BitMask>134217728</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Short Mark Line</TestName>
				<BitMask>268435456</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Fuse Blown</TestName>
				<BitMask>536870912</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
			<DeviceAlarmMapEntry>
				<TestName>Power Fail</TestName>
				<BitMask>1073741824</BitMask>
				<AlarmPriority>ba35e686-5cce-402d-982b-18d45958ccb6</AlarmPriority>
			</DeviceAlarmMapEntry>
		</DeviceAlarmMapEntryList>
		<ID>GSI2000 V1800 V1900 V6500 - Gauge Status</ID>
			<DeviceAlarmMapGuid>96F30116-EB23-4091-B150-F144991B9EEA</DeviceAlarmMapGuid>
			<NotAlarmText>Normal</NotAlarmText>
			<AlarmCategory>512ab266-b3b8-4a29-b8d9-594795cf63ed</AlarmCategory>
			<NormalUnacknowledgedPriority>5b7d7344-7d3c-4cde-a834-b5e2c8bfe11f</NormalUnacknowledgedPriority>
	</DeviceAlarmMap>
</ArrayOfDeviceAlarmMap>'


MERGE dbo.tblPointTemplate AS Target
USING 
( SELECT 'Standard Tank' as [ID],
				'' as [Description],
				1 as [Standard],
				NULL as [ExecutionInterval],
				27 as [LevelUnitIndex] ,
				2 as [TemperatureUnitIndex],
				191 as [DensityUnitIndex],
				73 as [PressureUnitIndex] ,
				109 as [FlowUnitIndex],
				46 as [VolumeUnitIndex],
				64 as [MassUnitIndex],
				162 as [VelocityUnitIndex],
				132 as [MassFlowUnitIndex],
				0 as [LevelDecimalPlaces],
				2 as [TemperatureDecimalPlaces],
				2 as [DensityDecimalPlaces],
				2 as [PressureDecimalPlaces],
				2 as [FlowDecimalPlaces],
				2 as [VolumeDecimalPlaces],
				2 as [MassDecimalPlaces],
				2 as [VelocityDecimalPlaces],
				2 as [MassFlowDecimalPlaces],
				40 as [LevelMaximum],
				0 as [LevelMinimum],
				300.0 as [TemperatureMaximum],
				-300.0 as [TemperatureMinimum],
				100 as [DensityMaximum],
				0 as [DensityMinimum],
				30.00 as [PressureMaximum],
				0 as [PressureMinimum],
				1000.00 as [VolumetricFlowMaximum],
				-1000.00 as [VolumetricFlowMinimum],
				10000.00 as [VolumeMaximum],
				0 as [VolumeMinimum],
				10000000 as [MassMaximum],
				0 as [MassMinimum],
				10 as [VelocityMaximum],
				-10 as [VelocityMinimum],
				3000 as [MassFlowMaximum],
				-3000 as [MassFlowMinimum],
				@PointTemplateGuid as [PointTemplateGuid],
				@TankSiteGuid as [SiteGuid],
				@ProfileImageGuid as [ProfileImageGuid],
				@DefaultDrawingGuid as [DefaultDrawingGuid],
				@StandardTankPointCommandStatus as [PointCommandStatus],
				@StandardTankDeviceAlarmMaps as [DeviceAlarmMaps],
				'2015-02-04' as [CreatedDate],
				'Administrator' as [CreatedBy],
				'2015-02-04' as [UpdatedDate],
				'Administrator' as [UpdatedBy]) 
AS Source
ON (Target.PointTemplateGuid = Source.PointTemplateGuid)
WHEN MATCHED THEN
	UPDATE SET		target.[ID]		= source.[ID],
						target.[UpdatedDate]		= SYSDATETIMEOFFSET(),
						target.[UpdatedBy]  = source.[UpdatedBy]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID], [Description], [Standard], [ExecutionInterval], [LevelUnitIndex], [TemperatureUnitIndex], [DensityUnitIndex], [PressureUnitIndex], [FlowUnitIndex],
				[VolumeUnitIndex], [MassUnitIndex], [VelocityUnitIndex], [MassFlowUnitIndex], [LevelDecimalPlaces], [TemperatureDecimalPlaces], [DensityDecimalPlaces],
				[PressureDecimalPlaces], [FlowDecimalPlaces], [VolumeDecimalPlaces], [MassDecimalPlaces], [VelocityDecimalPlaces], [MassFlowDecimalPlaces], [LevelMaximum],
				[LevelMinimum], [TemperatureMaximum], [TemperatureMinimum], [DensityMaximum], [DensityMinimum], [PressureMaximum], [PressureMinimum], [VolumetricFlowMaximum],
				[VolumetricFlowMinimum], [VolumeMaximum], [VolumeMinimum], [MassMaximum], [MassMinimum], [VelocityMaximum], [VelocityMinimum], [MassFlowMaximum],
				[MassFlowMinimum], [PointTemplateGuid], [SiteGuid], [ProfileImageGuid], [DefaultDrawingGuid], [PointCommandStatus], [DeviceAlarmMaps], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
	VALUES (Source.[ID], Source.[Description], Source.[Standard], Source.[ExecutionInterval], Source.[LevelUnitIndex], Source.[TemperatureUnitIndex], Source.[DensityUnitIndex], Source.[PressureUnitIndex], Source.[FlowUnitIndex],
				Source.[VolumeUnitIndex], Source.[MassUnitIndex], Source.[VelocityUnitIndex], Source.[MassFlowUnitIndex], Source.[LevelDecimalPlaces], Source.[TemperatureDecimalPlaces], Source.[DensityDecimalPlaces],
				Source.[PressureDecimalPlaces], Source.[FlowDecimalPlaces], Source.[VolumeDecimalPlaces], Source.[MassDecimalPlaces], Source.[VelocityDecimalPlaces], Source.[MassFlowDecimalPlaces], Source.[LevelMaximum],
				Source.[LevelMinimum], Source.[TemperatureMaximum], Source.[TemperatureMinimum], Source.[DensityMaximum], Source.[DensityMinimum], Source.[PressureMaximum], Source.[PressureMinimum], Source.[VolumetricFlowMaximum],
				Source.[VolumetricFlowMinimum], Source.[VolumeMaximum], Source.[VolumeMinimum], Source.[MassMaximum], Source.[MassMinimum], Source.[VelocityMaximum], Source.[VelocityMinimum], Source.[MassFlowMaximum],
				Source.[MassFlowMinimum], Source.[PointTemplateGuid], Source.[SiteGuid], Source.[ProfileImageGuid], Source.[DefaultDrawingGuid], Source.[PointCommandStatus], Source.[DeviceAlarmMaps], Source.[CreatedDate], Source.[CreatedBy], Source.[UpdatedDate], Source.[UpdatedBy]);

IF ((SELECT COUNT(*) FROM tblPointTemplate WHERE ID = 'Standard Tank') = 1)
BEGIN
	DECLARE @PointTypeGuid UNIQUEIDENTIFIER = (SELECT ApplicationStringGuid FROM tblApplicationString WHERE ID = 'Tank')

	UPDATE tblPointTemplate SET PointTemplateTypeApplicationStringGuid = @PointTypeGuid WHERE ID = 'Standard Tank' AND PointTemplateTypeApplicationStringGuid IS NULL
END

DECLARE @8240TduCommandStatusListGuid NVARCHAR(MAX) = '892faab4-5982-ed1e-9f4c-3644a1b1baa2'
DECLARE @8240TduCommandStatusListNode XML = 
'	<PointCommandStatusList>
		<CommandStatusListGuid>892faab4-5982-ed1e-9f4c-3644a1b1baa2</CommandStatusListGuid>
		<ID>8240 TDU</ID>
		<CommandStatusList>
			<CommandStatusElement>
				<Key>TFG OK</Key>
				<Value>0</Value>
			</CommandStatusElement>
			<CommandStatusElement>
				<Key>OFFLINE</Key>
				<Value>1</Value>
			</CommandStatusElement>
			<CommandStatusElement>
				<Key>COMMAND FAILURE</Key>
				<Value>2</Value>
			</CommandStatusElement>
			<CommandStatusElement>
				<Key>NO STRAP OR TEMPERATURE</Key>
				<Value>14</Value>
			</CommandStatusElement>
			<CommandStatusElement>
				<Key>NO STRAP</Key>
				<Value>6</Value>
			</CommandStatusElement>
		</CommandStatusList>
	</PointCommandStatusList>'

IF(0 =(SELECT  COUNT(*) FROM tblPointTemplate WHERE ID = 'Standard Tank' AND
		[PointCommandStatus].exist('/PointCommandStatus/CommandStatusLists/PointCommandStatusList/CommandStatusListGuid[text()=sql:variable("@8240TduCommandStatusListGuid")]') >0))
BEGIN
 UPDATE tblPointTemplate
 SET PointCommandStatus.modify('insert sql:variable("@8240TduCommandStatusListNode") as last  into (/PointCommandStatus/CommandStatusLists)[1]')
 WHERE ID = 'Standard Tank'
END

DECLARE @enableDisableGuid NVARCHAR(MAX) = '08b8308c-3794-3377-01cb-bb0ab7ca8478'

DECLARE @enableDisableNode XML = 
'<PointCommandStatusList>
	<CommandStatusListGuid>08b8308c-3794-3377-01cb-bb0ab7ca8478</CommandStatusListGuid>
	<ID>Enable / Disable</ID>
	<CommandStatusList>
	<CommandStatusElement>
		<Key>Enable</Key>
		<Value>61</Value>
	</CommandStatusElement>
	<CommandStatusElement>
		<Key>Disable</Key>
		<Value>54</Value>
	</CommandStatusElement>
	</CommandStatusList>
</PointCommandStatusList>'

IF(0 =(SELECT  COUNT(*) FROM tblPointTemplate WHERE ID = 'Standard Tank' AND
		[PointCommandStatus].exist('/PointCommandStatus/CommandStatusLists/PointCommandStatusList/CommandStatusListGuid[text()=sql:variable("@enableDisableGuid")]') >0))
BEGIN
 UPDATE tblPointTemplate
 SET PointCommandStatus.modify('insert sql:variable("@enableDisableNode") as last  into (/PointCommandStatus/CommandStatusLists)[1]')
 WHERE ID = 'Standard Tank'
END

DECLARE @strapTableSelectGuid NVARCHAR(MAX) = '12110352-cd14-4e48-8b67-8ac7afbb5a01'

DECLARE @strapTableSelectNode XML = 
'<PointCommandStatusList>
	<CommandStatusListGuid>12110352-cd14-4e48-8b67-8ac7afbb5a01</CommandStatusListGuid>
	<ID>Strap Table Select</ID>
	<CommandStatusList>
	<CommandStatusElement>
		<Key>Normal</Key>
		<Value>20</Value>
	</CommandStatusElement>
	<CommandStatusElement>
		<Key>Relaxed</Key>
		<Value>512</Value>
	</CommandStatusElement>
	</CommandStatusList>
</PointCommandStatusList>'

IF(0 =(SELECT  COUNT(*) FROM tblPointTemplate WHERE ID = 'Standard Tank' AND
		[PointCommandStatus].exist('/PointCommandStatus/CommandStatusLists/PointCommandStatusList/CommandStatusListGuid[text()=sql:variable("@strapTableSelectGuid")]') >0))
BEGIN
 UPDATE tblPointTemplate
 SET PointCommandStatus.modify('insert sql:variable("@strapTableSelectNode") as last  into (/PointCommandStatus/CommandStatusLists)[1]')
 WHERE ID = 'Standard Tank'
END

DECLARE @LevelTemplateTagGuid UNIQUEIDENTIFIER = '9EAB1A9F-2AA2-4EC9-AC60-7231345A974A'
DECLARE @WaterLevelTemplateTagGuid UNIQUEIDENTIFIER = '257B0B99-B1F0-4FD2-BC76-348AEE522A90'
DECLARE @StrapVolumeTemplateTagGuid UNIQUEIDENTIFIER = 'DCB0D63D-5F0A-4AB9-B454-7F124063ED47'
DECLARE @WaterStrapVolumeTemplateTagGuid UNIQUEIDENTIFIER = 'D5C390E3-EE24-41D8-96C0-43E5DFFF8CC5'
DECLARE @TemperatureProductTemplateTagGuid UNIQUEIDENTIFIER = '8624008F-D28C-496d-8578-7227E329E493'
DECLARE @TemperatureDensityTemplateTagGuid UNIQUEIDENTIFIER = 'D350FEAB-229A-4808-A7A1-76E552501B47'
DECLARE @TemperatureVaporTemplateTagGuid UNIQUEIDENTIFIER = 'C58C9B1C-6471-474F-A2B4-6D9D2A5B5E7B'
DECLARE @DensityProductObservedTemplateTagGuid UNIQUEIDENTIFIER = '5C6F08CC-1CEF-4AF6-B25C-CD12F3C82FB7'
DECLARE @DensityProductStandardTemplateTagGuid UNIQUEIDENTIFIER = '8F82ABFB-8ED8-4A4B-9424-672C8E74752A'
DECLARE @PressureVaporTemplateTagGuid UNIQUEIDENTIFIER = 'A3D5835E-5F79-4110-8BA9-B868949E6EB9'
DECLARE @VolCorForTempTemplateTagGuid UNIQUEIDENTIFIER = '9127882D-A34D-4465-A338-DB9BC7CF5D02'
DECLARE @VolCorForPressTemplateTagGuid UNIQUEIDENTIFIER = '46C7073B-546C-4D31-B1BF-642DF6CC74AC'
DECLARE @VolCorForPressTempTemplateTagGuid UNIQUEIDENTIFIER = '58ADEE26-8DEC-47D8-BB44-870DC5D8CDDF'
DECLARE @VolumeCorrectionFactorTemplateTagGuid UNIQUEIDENTIFIER = '72269896-29D3-4082-856D-812E8BD90319'
DECLARE @APICorrectionErrorTemplateTagGuid UNIQUEIDENTIFIER = '70EC0770-89B6-4EF1-847D-C97EC459E988'
DECLARE @GaugeCommandTemplateTagGuid UNIQUEIDENTIFIER = '9CC55B3E-F7FA-46DF-98B0-A5085B45821B'
DECLARE @GaugePositionTemplateTagGuid UNIQUEIDENTIFIER = '2F73595D-BFAA-4615-BCAB-9186633C0692'
DECLARE @GaugeStatusTemplateTagGuid UNIQUEIDENTIFIER = '634B6903-B281-4C78-85E5-39C7EDF7C9EE'
DECLARE @GaugeAlarm8130TemplateTagGuid UNIQUEIDENTIFIER = 'E844B757-BC80-45A8-8CD1-FC82EAB18882'
DECLARE @DensityGaugeProductTemplateTagGuid UNIQUEIDENTIFIER = '07A95F08-2794-480e-92BD-0FF62CD8F7F2'
DECLARE @EnableDisableCommandTemplateTagGuid UNIQUEIDENTIFIER = '5EBD6979-3595-4AF9-ACE5-15CCFBBB667C'

DECLARE @StandardTankGaugeCommandStatusValue XML =
	'<PointCommandStatusListReference xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
		<PointCommandStatusListGuid>af943503-0f3d-4c22-9966-9e3272c0f224</PointCommandStatusListGuid>
		<CurrentKey xsi:nil="true" />
		<CurrentValue xsi:nil="true" />
	</PointCommandStatusListReference>'
	
DECLARE @StandardTankEnableDisableCommandStatusValue XML =
	'<PointCommandStatusListReference xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
		<PointCommandStatusListGuid>08b8308c-3794-3377-01cb-bb0ab7ca8478</PointCommandStatusListGuid>
		<CurrentKey xsi:nil="true" />
		<CurrentValue xsi:nil="true" />
	</PointCommandStatusListReference>'

DECLARE @StandardTankStrapTableSelectCommandStatusValue XML =
	'<PointCommandStatusListReference xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
		<PointCommandStatusListGuid>12110352-cd14-4e48-8b67-8ac7afbb5a01</PointCommandStatusListGuid>
		<CurrentKey xsi:nil="true" />
		<CurrentValue xsi:nil="true" />
	</PointCommandStatusListReference>'

DECLARE @StandardTankGaugeAlarmValue XML =
	'<DeviceAlarmMapReference xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
		<DeviceAlarmMapGuid>0FA75BC6-97CC-4BED-ACFD-665EA6E7E062</DeviceAlarmMapGuid>
		<CurrentValue xsi:nil="true" />
	</DeviceAlarmMapReference>'

--New Alarm Specific Tag Guids
DECLARE @GaugeScanFailureAlarmTemplateTagGuid UNIQUEIDENTIFIER = 'C3647407-584D-4BF8-96A4-760EF6F05C40'
DECLARE @GaugeInputOutputFailureAlarmTemplateTagGuid UNIQUEIDENTIFIER = 'AD742CC4-2E40-405B-939D-9EA0E9F47EBD'
DECLARE @GaugeDeviceAlarmTemplateTagGuid UNIQUEIDENTIFIER = 'AC8C1740-F926-4F59-97CE-125F5D16ED53'
DECLARE @GaugeConfigurationChangeAlarmTemplateTagGuid UNIQUEIDENTIFIER = 'A06D1736-F876-42DF-8D6C-D061765DF0F3'
DECLARE @GaugeRaiseFailureAlarmTemplateTagGuid UNIQUEIDENTIFIER = '441DC0A6-59F5-451A-91FA-473548C432A2'
DECLARE @GaugeLowerFailureAlarmTemplateTagGuid UNIQUEIDENTIFIER = 'E95E16F0-3539-4B58-B8FE-E31DE7E1EE16'
DECLARE @GaugeUploadFailureAlarmTemplateTagGuid UNIQUEIDENTIFIER = '401040C1-013B-4006-AB33-B789CBFFCFCE'
DECLARE @GaugeDownloadFailureAlarmTemplateTagGuid UNIQUEIDENTIFIER = '9DF2A822-13B9-4990-AE9B-8BBAB0EFBDE8'
DECLARE @GaugeOverfillAlarmTemplateTagGuid UNIQUEIDENTIFIER = 'DD364C45-F917-4BE5-A30C-AE5F42546F43'

DECLARE @GaugeScanFailureLimitTemplateTagGuid UNIQUEIDENTIFIER = '808A5A22-63CA-48E4-AE56-F5BB17EAE0D7'
DECLARE @GaugeInputOutputFailureLimitTemplateTagGuid UNIQUEIDENTIFIER = '3E986FBC-1954-4547-AF5D-448F5FC0DBE5'
DECLARE @GaugeDeviceLimitTemplateTagGuid UNIQUEIDENTIFIER = '54D2B2B5-5FFC-433D-B8CC-7C03406EDA80'
DECLARE @GaugeConfigurationChangeLimitTemplateTagGuid UNIQUEIDENTIFIER = '9CF71006-A92A-47D5-A66F-877915BDBB09'
DECLARE @GaugeRaiseFailureLimitTemplateTagGuid UNIQUEIDENTIFIER = '54D9CA74-E7C8-47F6-918B-68125511FFAD'
DECLARE @GaugeLowerFailureLimitTemplateTagGuid UNIQUEIDENTIFIER = '226F4F36-D6DB-42A1-9D5D-A9C743ECE970'
DECLARE @GaugeUploadFailureLimitTemplateTagGuid UNIQUEIDENTIFIER = 'A36136A3-2FFE-4F02-9B5A-FA5DBC73A1D9'
DECLARE @GaugeDownloadFailureLimitTemplateTagGuid UNIQUEIDENTIFIER = 'DBF6F0D2-6A63-4C19-A4F6-B8BD6CD776E7'
DECLARE @GaugeOverfillLimitTemplateTagGuid UNIQUEIDENTIFIER = 'C68B9B56-2679-4BB3-80B9-93A8318DF07C'

DECLARE @LevelHiHiAlarmTemplateTagGuid UNIQUEIDENTIFIER =	'3250eff0-1b50-4533-8aab-8ffdc8c732ca'
DECLARE @LevelHighAlarmTemplateTagGuid UNIQUEIDENTIFIER =	'42454b6c-5197-4b55-a139-fadd057ee447'
DECLARE @LevelMaxOpAlarmTemplateTagGuid UNIQUEIDENTIFIER =	'1450304d-ed13-4c7a-b4a7-86ab58012599'
DECLARE @LevelMinOpAlarmTemplateTagGuid UNIQUEIDENTIFIER =	'83e8905e-037b-4608-9e00-d00060a87c9f'
DECLARE @LevelLowAlarmTemplateTagGuid UNIQUEIDENTIFIER =		'98276f47-228a-46a3-be53-cbc2d06aaf2a'
DECLARE @LevelLoLoAlarmTemplateTagGuid UNIQUEIDENTIFIER =	'2271b822-7531-4db8-8ec0-f97d9ea9d7f9'

DECLARE @LevelLowLimitTemplateTagGuid UNIQUEIDENTIFIER =		'a591c864-0421-4cb9-adf8-3d95598c02d2'
DECLARE @LevelLoLoLimitTemplateTagGuid UNIQUEIDENTIFIER =	'6e346a70-2ae0-4697-aab7-6e0ba21ad5b2'
DECLARE @LevelMinOpLimitTemplateTagGuid UNIQUEIDENTIFIER =	'79c8b5a4-30e7-4a4c-81be-38f82eb50bf3'
DECLARE @LevelHighLimitTemplateTagGuid UNIQUEIDENTIFIER =	'72711441-6dfb-4aff-b398-d5fcfa51f6d7'
DECLARE @LevelHiHiLimitTemplateTagGuid UNIQUEIDENTIFIER =	'6d127899-ac8f-4acc-bb16-8f1325980d70'
DECLARE @LevelMaxOpLimitTemplateTagGuid UNIQUEIDENTIFIER =	'11469d43-5c8b-492e-b166-272abfe7976a'

DECLARE @TemperatureHiHiAlarmTemplateTagGuid UNIQUEIDENTIFIER = 'DAD851BE-3642-4E8A-9702-6770D11A3DFB'
DECLARE @TemperatureHighAlarmTemplateTagGuid UNIQUEIDENTIFIER = '4CF27B1F-DC47-4D5B-84AD-0A1364C730A6'
DECLARE @TemperatureLowAlarmTemplateTagGuid UNIQUEIDENTIFIER = '9926DAC8-B394-469F-A381-2E295C9060CB'
DECLARE @TemperatureLoLoAlarmTemplateTagGuid UNIQUEIDENTIFIER = '3AA86977-B964-4FA3-945B-E7E9E3888E5C'

DECLARE @TemperatureHiHiLimitTemplateTagGuid UNIQUEIDENTIFIER = '7B640102-358F-4AB8-A141-0ED09FF2B3B6'
DECLARE @TemperatureHighLimitTemplateTagGuid UNIQUEIDENTIFIER = '633DD772-09C3-438B-A2EE-B970C348EC36'
DECLARE @TemperatureLowLimitTemplateTagGuid UNIQUEIDENTIFIER = '6BFFF2E8-DB93-424E-B3AC-D030942197BB'
DECLARE @TemperatureLoLoLimitTemplateTagGuid UNIQUEIDENTIFIER = '4C8335AF-F115-4B17-8546-B8B20226DB6A'

DECLARE @VolumeGrossObservedHiHiAlarmTemplateTagGuid UNIQUEIDENTIFIER = '6AC1D73D-67CA-4AF9-ACDF-AEA68841FA93'
DECLARE @VolumeGrossObservedHighAlarmTemplateTagGuid UNIQUEIDENTIFIER = '54116484-BAA3-4849-9091-502CF1FAE355'
DECLARE @VolumeGrossObservedLowAlarmTemplateTagGuid UNIQUEIDENTIFIER = 'D1850819-241E-4666-B4DB-2BCAC524CA3D'
DECLARE @VolumeGrossObservedLoLoAlarmTemplateTagGuid UNIQUEIDENTIFIER = '0F4E3644-27F3-4CB4-BF7F-674D72F6EC09'

DECLARE @VolumeGrossObservedHiHiLimitTemplateTagGuid UNIQUEIDENTIFIER = 'D31B9408-5095-4DD9-BBCE-17AA06B76D8A'
DECLARE @VolumeGrossObservedHighLimitTemplateTagGuid UNIQUEIDENTIFIER = '4F3B0D44-427C-4AC4-A3B7-4A665A2AEFEF'
DECLARE @VolumeGrossObservedLowLimitTemplateTagGuid UNIQUEIDENTIFIER = 'FF0E8916-0E9C-4AA1-AD1C-B0F787C676DD'
DECLARE @VolumeGrossObservedLoLoLimitTemplateTagGuid UNIQUEIDENTIFIER = '25D9D511-DD9C-41C0-969D-75A07D3A287A'

DECLARE @DensityHighAlarmTemplateTagGuid UNIQUEIDENTIFIER = 'FF555D32-A2DC-4F9A-9214-FAE78CD441A7'
DECLARE @DensityLowAlarmTemplateTagGuid UNIQUEIDENTIFIER = '6E847942-7381-4EB0-A670-08195503EFB5'

DECLARE @DensityHighLimitTemplateTagGuid UNIQUEIDENTIFIER = '67477744-599B-4DC2-95C9-700191E2C908'
DECLARE @DensityLowLimitTemplateTagGuid UNIQUEIDENTIFIER = '121B389F-B524-4761-91B5-31891E8B679F'

-- Quantity Module
DECLARE @GrossObservedVolumeTemplateTagGuid UNIQUEIDENTIFIER = '2A80E1CE-933F-44bd-B0ED-C3C2861CE89D'
DECLARE @RoofCorrectionTemplateTagGuid UNIQUEIDENTIFIER = '9173AD63-65C1-4c43-BB5A-054FFEABF1FE'
DECLARE @BottomVolumeTemplateTagGuid UNIQUEIDENTIFIER = 'C48D535A-2082-4c7b-AC91-93D17D89ADD8'
DECLARE @PercentBSWTemplateTagGuid UNIQUEIDENTIFIER = 'D723F705-01F6-4c46-92FF-BF46AB9A4C62'
DECLARE @TankShellCorrectionTemplateTagGuid UNIQUEIDENTIFIER = '526A0F15-EF35-45bc-8F3D-CD2DF565A649'
DECLARE @NetStandardVolumeTemplateTagGuid UNIQUEIDENTIFIER = '9A467CD0-6F77-4541-BEF3-C2C7E8879F05'
DECLARE @MassTemplateTagGuid UNIQUEIDENTIFIER = 'D4BD1CE3-D5E4-45b0-98A0-F8E5240E2A64'
DECLARE @SolidsVolumeTemplateTagGuid UNIQUEIDENTIFIER = '78100E99-0E39-4170-BB01-19F752F0D929'
DECLARE @DensityInAirTemplateTagGuid UNIQUEIDENTIFIER = '7F820D6E-D913-4258-8218-D0A68A8C4590'
DECLARE @StdDensityInAirTemplateTagGuid UNIQUEIDENTIFIER = '78B4749F-F02B-489c-9B0C-46C2F9E02116'
DECLARE @GrossStdWeightTemplateTagGuid UNIQUEIDENTIFIER = 'F640ABC2-F3B7-471a-834A-D231ACE7DC2A'
DECLARE @NetStdWeightTemplateTagGuid UNIQUEIDENTIFIER = 'EF176082-14E0-4a30-8FCC-53465F70E897'
DECLARE @GrossStdVolumeTemplateTagGuid UNIQUEIDENTIFIER = '76699AB2-813B-45f9-9F85-3DE09F22D6DD'
DECLARE @BSWVolumeTemplateTagGuid UNIQUEIDENTIFIER = '13EE61B5-5420-4CC1-9A6C-56DB3022423E'
DECLARE @TotalCalculatedVolumeTemplateTagGuid UNIQUEIDENTIFIER = '0434B936-4328-4BCE-A4DE-9BEFC2307437'

-- Shell Correction Module
DECLARE @TemperatureAmbientTemplateTagGuid UNIQUEIDENTIFIER = 'AF72ED89-FA23-446D-A551-16A915C8E0E9'

-- Roof correction module
DECLARE @CritizalZoneTemplateTagGuid UNIQUEIDENTIFIER = 'DBF87C9C-65DB-41fe-AAB0-37129382866F'

-- solids level
DECLARE @SolidsLevelTemplateTagGuid UNIQUEIDENTIFIER = '1C9EA4B3-5460-450d-8971-D97CD0E43280'

-- Tank Command Module
DECLARE @TankCommandTagGuid UNIQUEIDENTIFIER = '99164989-A1DB-4C83-A834-01396B8D589E'
DECLARE @TankStatusTagGuid  UNIQUEIDENTIFIER = 'EDD65B84-474F-4CD9-B169-42517668338C'
DECLARE @VolumeTotalObservedRateTagGuid  UNIQUEIDENTIFIER = 'F7B61B07-7364-4DBF-82C8-6EDC1E7C6E21'
DECLARE @VolumeNetStandardRateTagGuid  UNIQUEIDENTIFIER = '2D928F24-9B54-40D5-B00B-D19D87069D74'
DECLARE @LevelProductRateTagGuid  UNIQUEIDENTIFIER = '70ADFA50-CCCA-4ABC-8055-5889F4433E26'
DECLARE @VolumeGrossObservedRateTagGuid  UNIQUEIDENTIFIER = '66BDC1EB-98F8-4179-B54F-E9CBCA9D8DE0'
DECLARE @LevelProductStopTagGuid  UNIQUEIDENTIFIER = 'B9E03854-EDDC-4165-8A11-6FD56B79E988'
DECLARE @LevelProductMovementTagGuid  UNIQUEIDENTIFIER = 'A7B30ADC-FBF2-4F35-94A6-ED75B9E5E062'
DECLARE @TankModeDiscreteAlarmTagGuid  UNIQUEIDENTIFIER = '3E0B375C-F090-430A-BC16-A4C9883D0F13'
DECLARE @TankModeAlarmTagGuid  UNIQUEIDENTIFIER = '6C10761C-9FAA-4525-BA7C-66E64E2AB583'
DECLARE @TankModeDiscreteAlarmMovementTestTagGuid UNIQUEIDENTIFIER = '1326309F-2EA5-4232-879F-FEBB34E907F0'
DECLARE @TankModeDiscreteAlarmReverseFlowTestTagGuid UNIQUEIDENTIFIER = 'AED30BF3-9BDA-40BE-A350-17488F9460A4'
DECLARE @TankModeDiscreteAlarmNoFlowTestTagGuid UNIQUEIDENTIFIER = '410FC003-95B5-4D03-A96A-0605BFA3E38B'
DECLARE @TankModeDiscreteAlarmTesttingTestTagGuid UNIQUEIDENTIFIER = '30671089-5795-4841-8A68-1796A22248F1'


-- Available Volume / Remaining Capacity
DECLARE @GrossObservedVolumeAvailableTagGuid  UNIQUEIDENTIFIER = 'EA90B57C-E223-4041-95C0-8BC15A097755'
DECLARE @GrossObservedVolumeRemainingTagGuid  UNIQUEIDENTIFIER = 'ABAFCCD1-1480-47A4-81AF-2D86413DD27D'
DECLARE @NetStandardVolumeAvailableTagGuid  UNIQUEIDENTIFIER = '816A00C8-FCD7-4D79-A17C-BD8C8F0AD2DC'
DECLARE @NetStandardVolumeRemainingTagGuid UNIQUEIDENTIFIER = '8F35B5D4-CA96-47FA-8A76-AD99326B9D19'

-- Transfer Module
DECLARE @TransferModeTemplateTagGuid  UNIQUEIDENTIFIER = 'B24E6A16-AB76-4980-A648-07A724D84A74'
DECLARE @TransferStatusTemplateTagGuid UNIQUEIDENTIFIER = '7A451A61-E2E6-480E-AE85-609E7BC2A57F'
DECLARE @TransferTargetSetPointTemplateTagGuid UNIQUEIDENTIFIER = 'CF589E1E-5BBE-49F5-8381-3160709E2889'
DECLARE @TransferLevelStartTemplateTagGuid UNIQUEIDENTIFIER = '12F92CBC-BEA9-472D-87B2-34D2A838647C'
DECLARE @TransferStartGOVTemplateTagGuid UNIQUEIDENTIFIER = 'DC7BFBCA-4F68-46EB-92B4-3921B6E13019'
DECLARE @TransferStartNSVTemplateTagGuid UNIQUEIDENTIFIER = '8678EA00-E579-4A04-9DC4-F655731DCE3C'
DECLARE @TransferStartVolumeWaterTemplateTagGuid UNIQUEIDENTIFIER = '3D8C66F7-E4B7-4CD4-91B1-65C8FE050EBD'
DECLARE @TransferStartVolumeTemplateTagGuid UNIQUEIDENTIFIER = 'F270261C-C376-4617-98FA-37C67A4C1019'
DECLARE @TransferTimeRemainingTemplateTagGuid UNIQUEIDENTIFIER = '37A589E6-4230-4A9F-8044-AA66C7BED7A7'
DECLARE @TransferTimeCompletionTemplateTagGuid UNIQUEIDENTIFIER = 'C34BC94B-F8E8-41ED-80E4-5FEB10094785'
DECLARE @TransferredGOVTemplateTagGuid UNIQUEIDENTIFIER = 'EE65D3E3-818F-4304-846C-AB711471185C'
DECLARE @TransferredNSVTemplateTagGuid UNIQUEIDENTIFIER = 'B7043057-E2C9-4BD3-848C-04FD0CD6E0A7'		
DECLARE @TransferredVolumeWaterTemplateTagGuid UNIQUEIDENTIFIER = '06DA0DAF-1227-4E3B-85CA-689A95101060'		
DECLARE @TransferredVolumeTemplateTagGuid UNIQUEIDENTIFIER = 'B9361FF1-4D5F-44C8-AA36-8302F26E2BEE'
DECLARE @TankTransferDiscreteAlarmTemplateTagGuid UNIQUEIDENTIFIER = 'C2400FDD-55BB-4E43-ABB5-7D87B510513A'
DECLARE @TransferAdvisoryAlarmTemplateTagGuid UNIQUEIDENTIFIER = 'F4771CFC-9D94-405C-9388-0C3946C7136B'
DECLARE @TransferTargetAlarmTemplateTagGuid UNIQUEIDENTIFIER = '24D1A77D-ADB8-4013-92A5-D917B00F829A'
DECLARE @TransferAdvisoryAlarmLimitTemplateTagGuid UNIQUEIDENTIFIER = 'BBA3B93B-9320-4610-B749-369D6DC9FCAD'
DECLARE @TransferShutdownAlarmLimitTemplateTagGuid UNIQUEIDENTIFIER = '799C30DF-12AF-4EEB-BC8D-A5A2A71BACF8'
DECLARE @TransferStartTimeTagGuid UNIQUEIDENTIFIER = 'F60377C7-DD21-4ECA-B22C-BE2F6950C85E'
DECLARE @TransferStopTimeTagGuid UNIQUEIDENTIFIER = '349FAEA0-766A-4C20-BF1F-7CB85E7BE1FC'
DECLARE @TransferLevelTargetTagGuid UNIQUEIDENTIFIER = '64710B23-C502-4513-8C7C-8B7812ABA684'
DECLARE @TransferVolumeTargetTagGuid UNIQUEIDENTIFIER = '86A99CD8-0235-4CC5-BDA4-F5E4CBD15894'

-- Level Rate Alarm Tag GUIDs
DECLARE @LevelProductRateAlarmEmptyingTemplateTagGuid		UNIQUEIDENTIFIER = '115D092F-F02C-4232-8A03-D282966EFC5E'
DECLARE @LevelProductRateAlarmFillingTemplateTagGuid		UNIQUEIDENTIFIER = 'DA6227DA-6897-479F-90B7-756168807EE4'
DECLARE @LevelProductRateAlarmLimitEmptyingTemplateTagGuid	UNIQUEIDENTIFIER = '8CD9CA60-A64E-4783-BFC4-F247B362324C'
DECLARE @LevelProductRateAlarmLimitFillingTemplateTagGuid	UNIQUEIDENTIFIER = '19EC34C8-0BED-4736-BF27-3BD2CE42AB39'

-- Flow Rate TOV (Strap Volume) Alarm Tag GUIDs
DECLARE @VolumeTotalObservedRateAlarmEmptyingTemplateTagGuid		UNIQUEIDENTIFIER = 'AEF2C1E3-E376-46F0-B788-C7BED8EDFB6B'
DECLARE @VolumeTotalObservedRateAlarmFillingTemplateTagGuid			UNIQUEIDENTIFIER = 'A2802F87-FE5E-44E5-BBDB-7BFEA99A20F8'
DECLARE @VolumeTotalObservedRateAlarmLimitEmptyingTemplateTagGuid	UNIQUEIDENTIFIER = 'F57B8450-63A5-4D87-8250-495FDC7E8C60'
DECLARE @VolumeTotalObservedRateAlarmLimitFillingTemplateTagGuid	UNIQUEIDENTIFIER = '87880CA4-0021-4919-9702-F58869D3E815'

-- Flow Rate NSV Alarm Tag GUIDs
DECLARE @VolumeNetStandardRateAlarmEmptyingTemplateTagGuid		UNIQUEIDENTIFIER = '80A0B35B-61F8-44E0-89D1-29F866907ED0'
DECLARE @VolumeNetStandardRateAlarmFillingTemplateTagGuid		UNIQUEIDENTIFIER = 'F4D8E832-C768-4977-A96B-1B9E15B38238'
DECLARE @VolumeNetStandardRateAlarmLimitEmptyingTemplateTagGuid	UNIQUEIDENTIFIER = '920602D6-8A67-4B0E-8AF1-52816C31511E'
DECLARE @VolumeNetStandardRateAlarmLimitFillingTemplateTagGuid	UNIQUEIDENTIFIER = '5A76A99F-CD19-4851-B8AF-AC9C4B9928D0'

-- Flow Rate GOV Alarm Tag GUIDs
DECLARE @VolumeGrossObservedRateAlarmEmptyingTemplateTagGuid		UNIQUEIDENTIFIER = 'F02D09D0-7339-4FF8-8433-809819BCD94B'
DECLARE @VolumeGrossObservedRateAlarmFillingTemplateTagGuid			UNIQUEIDENTIFIER = '16446DB3-16E2-454F-8C64-AA6E234B9580'
DECLARE @VolumeGrossObservedRateAlarmLimitEmptyingTemplateTagGuid	UNIQUEIDENTIFIER = '9BCCC929-775E-4366-88D4-477CF819A34D'
DECLARE @VolumeGrossObservedRateAlarmLimitFillingTemplateTagGuid	UNIQUEIDENTIFIER = '03824C96-FAE9-4A0D-8571-0973A2EF0321'

-- vapor tags
DECLARE @DensityVaporTemplateTagGuid		UNIQUEIDENTIFIER = '9BFEBBE8-BF75-430b-8F79-0DE0AA6DD430'
DECLARE @VolumeVaporNetTemplateTagGuid		UNIQUEIDENTIFIER = 'AE0EF9EC-9D71-4e57-929E-4ED668026383'
DECLARE @MassVaporTemplateTagGuid			UNIQUEIDENTIFIER = '0B5FB637-24BF-428a-9152-E327719F6B5E'

-- miscelaneous tags
DECLARE @OperationalModeTemplateTagGuid UNIQUEIDENTIFIER = '257F2C81-D6F3-4268-A756-E4B8C56088AD'

-- Leak Detection tags
DECLARE @LeakDetectionAlarmTagGuid uniqueidentifier = '733963D9-1FE7-45CB-B362-00B2D35A95AD'
DECLARE @LeakDetectionDataInsufficientLimitTagGuid uniqueidentifier = '45F7293C-1907-42C9-A929-9BC246727117'
DECLARE @LeakDetectionDiscreteAlarmTagGuid uniqueidentifier = '1FA8E5CC-B11C-46A2-B40F-C0907B64901E'
DECLARE @LeakRateTagGuid uniqueidentifier = '5F541EB7-C6A3-477B-8E8E-3C8A3B9F53B4'
DECLARE @LeakRateHighAlarmTagGuid uniqueidentifier = '1B5DA8D0-8880-41A4-AC0C-D915F2BF0593'
DECLARE @LeakRateHighLimitTagGuid uniqueidentifier = '8228F563-D0B7-4445-A7BF-F123F2504EB6'
DECLARE @PressureBottomTagGuid uniqueidentifier = 'F27E3FEB-E180-4F6A-81F9-11C09FD17812'
DECLARE @VolumeCorrectionFactorUnroundedTagGuid uniqueidentifier = 'D83854E0-41F4-474A-BDF4-21D2172065A4'
DECLARE @VolumeNetStandardUnroundedTagGuid uniqueidentifier = '34CF4D6F-B832-4C8D-82F4-9AE591CA2740'
DECLARE @LeakDetectionDataLastRunTimeTagGuid uniqueidentifier = '18028EDA-1E50-4090-89A2-E99EA25EA2214'

DECLARE @NumberStrapPointTagGuid uniqueidentifier = '6EC3441F-4FE0-43F9-B61C-5AE021E5DFBA'
DECLARE @StrapTableSelectTagGuid uniqueidentifier = '2152340B-D8E2-4617-A942-00BD0575F8E7'
DECLARE @PressureGaugeFilterTagGuid uniqueidentifier = '064CAD8A-B129-4F87-9563-75D6B2552FDD'

--Create Tags
MERGE dbo.tblPointTemplateTag AS Target
USING 
(SELECT 'Level Product' as [ID], 
				3 as [EngineeringUnitsType],  -- enum EngineeringUnitType {FmuAll=0, FmuTemp=1, FmuTime=2, FmuLength=3, FmuArea=4, FmuVolume=5, FmuMass=6, FmuPressure=7, FmuVolflow=8, FmuMassflow=9, FmuVelocity=10, FmuDensity=11, FmuEnergy=12, FmuPower=13, FmuElect=14, FmuNodim=15, FmuNone=16}
				27 as [EngineeringUnitsIndex],
				0 as [DecimalPlaces],
				27 as [ServerEngineeringUnitsIndex],
				'System.Double' as [ValueType],
				NULL as [Value],
				40 as [Maximum],
				0 as [Minimum],
				1 as [PointTagInputOutputTypeIndex],  -- enum PointTagInputOutputType {UnAssigned = 0, Manual = 1, Calculated = 2, OpcUa = 3, FCEE = 4}
				1 as [Input],
				0 as [AlarmStatus],
				1 as [ApplyPointTemplateEngineeringUnits],
				1 as [ApplyPointTemplateDecimalPlaces],
				1 as [ApplyPointTemplateMaximum],
				1 as [ApplyPointTemplateMinimum],
				@LevelTemplateTagGuid as [PointTemplateTagGuid],
				@PointTemplateGuid as [PointTemplateGuid],
				@LevelProductWellKnownGuid as [WellKnownIdentityGuid],
				1 as [AlarmsEnabled],
				0 as [InhibitInputOutputTypeConfiguration],
				0 as [InhibitOverride],
				0 as [Module],
				1 as [Archived],
				'2015-02-04' as [CreatedDate],
				'Administrator' as [CreatedBy],
				'2015-02-04' as [UpdatedDate],
				'Administrator' as [UpdatedBy]	
		UNION ALL
		SELECT 'Level Water' ,3,27,0,27,'System.Double','<double>0.0</double>',40,0,1,1,0,1,1,1,1,@WaterLevelTemplateTagGuid,@PointTemplateGuid,@LevelWaterWellKnownGuid,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Volume Total Observed' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@StrapVolumeTemplateTagGuid,@PointTemplateGuid,@VolumeTotalObservedWellKnownGuid,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Volume Water' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@WaterStrapVolumeTemplateTagGuid,@PointTemplateGuid,@VolumeWaterWellKnownGuid,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'

		-- Tags for VCF Module
		-- Temperature Product - input
		UNION ALL
		SELECT 'Temperature Product' ,1,2,2,2,'System.Double',NULL,300.0,-300.0,1,1,0,1,1,1,1,@TemperatureProductTemplateTagGuid,@PointTemplateGuid,@TemperatureProductWellKnownGuid,1,0,0,0,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Temperature Density - input
		UNION ALL
		SELECT 'Temperature Density' ,1,2,2,2,'System.Double',NULL,300.0,-300.0,0,1,0,1,1,1,1,@TemperatureDensityTemplateTagGuid,@PointTemplateGuid,@TemperatureDensityWellKnownGuid,1,0,0,0,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Temperature Vapor - input
		UNION ALL
		SELECT 'Temperature Vapor' ,1,2,2,2,'System.Double',NULL,300.0,-300.0,1,1,0,1,1,1,1,@TemperatureVaporTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Density Product Observed - input
		UNION ALL
		SELECT 'Density Product Observed' ,11,191,2,191,'System.Double',NULL,100.0,0.0,2,1,0,1,1,1,1,@DensityProductObservedTemplateTagGuid,@PointTemplateGuid,@DensityProductObservedWellKnownGuid,1,0,0,0,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Density Product Standard - input
		UNION ALL
		SELECT 'Density Product Standard' ,11,191,2,191,'System.Double',NULL,100.0,0.0,1,1,0,1,1,1,1,@DensityProductStandardTemplateTagGuid,@PointTemplateGuid,@DensityProductStandardWellKnownGuid ,1,0,0,0,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Density Product in Air - input
		UNION ALL
		SELECT 'Density Product in Air' ,11,191,2,191,'System.Double',NULL,100.0,0,2,1,0,1,1,1,1,@DensityInAirTemplateTagGuid ,@PointTemplateGuid,@DensityProductInAirWellKnownGuid,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		-- Density Product in Air Standard - input
		UNION ALL
		SELECT 'Density Product Standard in Air' ,11,191,2,191,'System.Double',NULL,100.0,0,2,1,0,1,1,1,1,@StdDensityInAirTemplateTagGuid ,@PointTemplateGuid,@DensityProductStandardInAirWellKnownGuid,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		-- Pressure Vapor - input
		UNION ALL
		SELECT 'Pressure Vapor' ,7,73,2,73,'System.Double',NULL,30.0,0.0,1,1,0,1,1,1,1,@PressureVaporTemplateTagGuid,@PointTemplateGuid,@PressureVaporWellKnownGuid ,1,0,0,0,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Volume Correction for Temperature - ctl - output
		UNION ALL
		SELECT 'Volume Correction for Temperature' ,15,255,5,255,'System.Double',NULL,2.0,0.0,2,1,0,1,0,0,0,@VolCorForTempTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Volume Correction for Pressure - cpl - output
		UNION ALL
		SELECT 'Volume Correction for Pressure' ,15,255,5,255,'System.Double',NULL,2.0,0.0,2,1,0,1,0,0,0,@VolCorForPressTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Volume Correction for Temp and Press - ctpl - output
		UNION ALL
		SELECT 'Volume Correction for Temperature and Pressure' ,15,255,5,255,'System.Double',NULL,2.0,0.0,2,1,0,1,0,0,0,@VolCorForPressTempTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Volume Correction Factor - VCF UnRounded - output
		UNION ALL
		SELECT 'Volume Correction Factor' ,15,255,5,255,'System.Double',NULL,2.0,0.0,2,1,0,1,0,0,0,@VolumeCorrectionFactorTemplateTagGuid,@PointTemplateGuid,@VolumeCorrectionFactorWellKnownGuid,1,0,0,0,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- API Correction Error - output
		UNION ALL
		SELECT 'API Correction Error' ,16,255,0,255,'System.Boolean',NULL,1.0,0.0,2,1,0,1,1,1,1,@APICorrectionErrorTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Gauge Command - output
		UNION ALL
		SELECT 'Gauge Command' ,16,255,0,255,'FMBusinessObjects.DataObjects.PointCommandStatusListReference',@StandardTankGaugeCommandStatusValue,9999.0,0.0,1,0,0,1,1,1,1,@GaugeCommandTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Gauge Status - input
		UNION ALL
		SELECT 'Gauge Status' ,16,255,0,255,'FMBusinessObjects.DataObjects.PointCommandStatusListReference',@StandardTankGaugeCommandStatusValue,9999.0,0.0,3,1,0,1,1,1,1,@GaugeStatusTemplateTagGuid,@PointTemplateGuid,null,0,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Gauge Position - output
		UNION ALL
		SELECT 'Gauge Position' ,3,27,0,27,'System.Double',NULL,40,0,3,1,0,1,1,1,1,@GaugePositionTemplateTagGuid,@PointTemplateGuid,null,0,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		-- Gauge Alarm - input
		UNION ALL
		SELECT 'Gauge Alarm' ,16,255,0,255,'FMBusinessObjects.DataObjects.DeviceAlarmMapReference',@StandardTankGaugeAlarmValue,4294967295.0,0.0,3,1,0,1,1,1,1,@GaugeAlarm8130TemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- Density Gauge - input
		UNION ALL
		SELECT 'Density Product Gauge' ,11,191,2,191,'System.Double',NULL,100.0,0.0,0,1,0,1,1,1,1,@DensityGaugeProductTemplateTagGuid,@PointTemplateGuid,null ,1,0,0,0,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		-- New Alarm Tags
		UNION ALL
		SELECT 'Level Product Low Limit' ,3,27,0,27,'System.Double','<double>4.0</double>',40,0,1,1,0,1,1,1,1,@LevelLowLimitTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Level Product LoLo Limit' ,3,27,0,27,'System.Double','<double>2.0</double>',40,0,1,1,0,1,1,1,1,@LevelLoLoLimitTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Level Product Min Op Limit' ,3,27,0,27,'System.Double','<double>6.0</double>',40,0,1,1,0,1,1,1,1,@LevelMinOpLimitTemplateTagGuid,@PointTemplateGuid,@LevelProductMinOpLimitWellKnownGuid,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Level Product High Limit' ,3,27,0,27,'System.Double','<double>36.0</double>',40,0,1,1,0,1,1,1,1,@LevelHighLimitTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Level Product HiHi Limit' ,3,27,0,27,'System.Double','<double>38.0</double>',40,0,1,1,0,1,1,1,1,@LevelHiHiLimitTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Level Product Max Op Limit' ,3,27,0,27,'System.Double','<double>34.0</double>',40,0,1,1,0,1,1,1,1,@LevelMaxOpLimitTemplateTagGuid,@PointTemplateGuid,@LevelProductMaxOpLimitWellKnownGuid,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'


		UNION ALL
		SELECT 'Level Product HiHi Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@LevelHiHiAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Level Product High Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@LevelHighAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Level Product Max Op Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@LevelMaxOpAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Level Product Min Op Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@LevelMinOpAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Level Product Low Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@LevelLowAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Level Product LoLo Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@LevelLoLoAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'

		-- Temperature Alarm Tags
		UNION ALL
		SELECT 'Temperature Product Low Limit' ,1,2,2,2,'System.Double','<double>-240.0</double>',300.0,-300.0,1,1,0,1,1,1,1,@TemperatureLowLimitTemplateTagGuid,@PointTemplateGuid,@TemperatureProductLowWellKnownGuid,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Temperature Product LoLo Limit' ,1,2,2,2,'System.Double','<double>-270.0</double>',300.0,-300.0,1,1,0,1,1,1,1,@TemperatureLoLoLimitTemplateTagGuid,@PointTemplateGuid,@TemperatureProductLoLoWellKnownGuid,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Temperature Product High Limit' ,1,2,2,2,'System.Double','<double>240.0</double>',300.0,-300.0,1,1,0,1,1,1,1,@TemperatureHighLimitTemplateTagGuid,@PointTemplateGuid,@TemperatureProductHighWellKnownGuid,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Temperature Product HiHi Limit' ,1,2,2,2,'System.Double','<double>270.0</double>',300.0,-300.0,1,1,0,1,1,1,1,@TemperatureHiHiLimitTemplateTagGuid,@PointTemplateGuid,@TemperatureProductHiHiWellKnownGuid,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'


		UNION ALL
		SELECT 'Temperature Product HiHi Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@TemperatureHiHiAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Temperature Product High Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@TemperatureHighAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Temperature Product Low Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@TemperatureLowAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Temperature Product LoLo Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@TemperatureLoLoAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'

		-- Density Alarm Tags
		UNION ALL
		SELECT 'Density Product Observed Low Limit' ,11,191,2,191,'System.Double','<double>5.0</double>',100.0,0.0,1,1,0,1,1,1,1,@DensityLowLimitTemplateTagGuid,@PointTemplateGuid,@DensityProductLowWellKnownGuid ,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Density Product Observed High Limit' ,11,191,2,191,'System.Double','<double>95.0</double>',100.0,0.0,1,1,0,1,1,1,1,@DensityHighLimitTemplateTagGuid,@PointTemplateGuid,@DensityProductHighWellKnownGuid ,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'


		UNION ALL
		SELECT 'Density Product Observed High Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@DensityHighAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Density Product Observed Low Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@DensityLowAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'


		-- Tags for Quantity Module
		UNION ALL
		SELECT 'Volume Gross Observed' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@GrossObservedVolumeTemplateTagGuid ,@PointTemplateGuid,@VolumeGrossObservedWellKnownGuid ,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Volume Roof Correction' ,5,46,2,46,'System.Double',NULL,2000.0,-2000.0,2,1,0,1,1,0,0,@RoofCorrectionTemplateTagGuid ,@PointTemplateGuid,@VolumeRoofCorrectionWellKnownGuid,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Volume Bottoms' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@BottomVolumeTemplateTagGuid ,@PointTemplateGuid,null,1,0,0,0,0,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Percent BSW' ,15,255,1,255,'System.Double','<double>0.0</double>',100.0,0,1,1,0,1,0,0,0,@PercentBSWTemplateTagGuid ,@PointTemplateGuid, @PercentBSWWellKnownGuid,1,0,0,0,1,'2015-02-04','Administrator','2025-10-28','Administrator'
		UNION ALL
		SELECT 'Tank Shell Correction' ,15,255,15,255,'System.Double',NULL,2.0,0,2,1,0,1,0,0,0,@TankShellCorrectionTemplateTagGuid ,@PointTemplateGuid,@TankShellCorrectionWellKnownGuid,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Volume Net Standard' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@NetStandardVolumeTemplateTagGuid ,@PointTemplateGuid,@VolumeNetStandardWellKnownGuid,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Mass Liquid' ,6,64,2,64,'System.Double',NULL,10000000.0,0,2,1,0,1,1,1,1,@MassTemplateTagGuid ,@PointTemplateGuid,@MassLiquidWellKnownGuid ,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Volume Solids' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@SolidsVolumeTemplateTagGuid ,@PointTemplateGuid,null,1,0,0,0,0,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Weight Gross Standard' ,6,64,2,64,'System.Double',NULL,10000000.0,0,2,1,0,1,1,1,1,@GrossStdWeightTemplateTagGuid ,@PointTemplateGuid,null,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Weight Net Standard' ,6,64,2,64,'System.Double',NULL,10000000.0,0,2,1,0,1,1,1,1,@NetStdWeightTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Volume Gross Standard' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@GrossStdVolumeTemplateTagGuid ,@PointTemplateGuid,@VolumeGrossStandardWellKnownGuid,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Volume BSW' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@BSWVolumeTemplateTagGuid ,@PointTemplateGuid,@VolumeBSWWellKnownGuid,1,0,0,0,1,'2015-02-04','Administrator','2025-10-28','Administrator'
		UNION ALL
		SELECT 'Volume Total Calculated' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@TotalCalculatedVolumeTemplateTagGuid ,@PointTemplateGuid,null,1,0,0,0,0,'2015-02-04','Administrator','2015-02-04','Administrator'


		-- Tag for Shell Correction Module
		UNION ALL
		SELECT 'Temperature Ambient' ,1,2,2,2,'System.Double',NULL,300.0,-300.0,1,1,0,1,1,1,1,@TemperatureAmbientTemplateTagGuid,@PointTemplateGuid,@TemperatureAmbientWellKnownGuid,1,0,0,0,1,'2016-05-31','Administrator','2016-05-31','Administrator'

		-- Tag for roof correction
		UNION ALL
		SELECT 'Roof Critical Zone' ,16,255,0,255,'System.Boolean',NULL,1.0,0.0,2,1,0,1,1,1,1,@CritizalZoneTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'

		-- Solids Level
		UNION ALL
		SELECT 'Level Solids' ,3,27,0,27,'System.Double','<double>0.0</double>',40,0,1,1,0,1,1,1,1,@SolidsLevelTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2015-02-04','Administrator','2015-02-04','Administrator'

		-- tag for Tank Command
		UNION ALL
		SELECT 'Tank Command' ,16,255,0,255,'FMBusinessObjects.DataObjects.CodedVariables.TankCommands','<TankCommands>Stop</TankCommands>',5.0,0.0,1,0,0,1,1,1,1,@TankCommandTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Tank Status' ,16,255,0,255,'FMBusinessObjects.DataObjects.CodedVariables.TankStatuses','<TankStatuses>Stopped</TankStatuses>',4.0,0.0,2,0,0,1,1,1,1,@TankStatusTagGuid,@PointTemplateGuid,@TankStatusWellKnownGuid,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Volume Total Observed Rate' ,8,109,2,109,'System.Double',NULL,1000.0,-1000.0,2,1,0,1,1,1,1,@VolumeTotalObservedRateTagGuid,@PointTemplateGuid,@VolumeTotalObservedRateWellKnownGuid,1,0,0,0,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Volume Net Standard Rate' ,8,109,2,109,'System.Double',NULL,1000.0,-1000.0,2,1,0,1,1,1,1,@VolumeNetStandardRateTagGuid,@PointTemplateGuid,@VolumeNetStandardRateWellKnownGuid,1,0,0,0,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Level Product Rate' ,10,162,2,162,'System.Double',NULL,10.0,-10.0,2,1,0,1,1,1,1,@LevelProductRateTagGuid,@PointTemplateGuid,null,1,0,0,0,1,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Volume Gross Observed Rate' ,8,109,2,109,'System.Double',NULL,1000.0,-1000.0,2,1,0,1,1,1,1,@VolumeGrossObservedRateTagGuid,@PointTemplateGuid,@VolumeGrossObservedRateWellKnownGuid,1,0,0,0,1,'2017-08-30','Administrator','2017-08-30','Administrator'
		UNION ALL
		SELECT 'Level Product Stop' ,3,27,2,27,'System.Double',NULL,40.0,0.0,2,1,0,1,1,1,1,@LevelProductStopTagGuid,@PointTemplateGuid,null,1,1,1,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Level Product Movement' ,3,27,2,27,'System.Double',NULL,40.0,0.0,2,1,0,1,1,1,1,@LevelProductMovementTagGuid,@PointTemplateGuid,null,1,1,1,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Tank Mode Discrete Alarm' ,16,255,0,255,'System.Int16',NULL,255.0,0.0,2,1,0,1,1,1,1,@TankModeDiscreteAlarmTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Tank Mode Alarm' ,16,255,0,255,'System.String',NULL,0.0,0.0,2,1,0,1,1,1,1,@TankModeAlarmTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Tank Mode Discrete Alarm Movement Limit' ,16,255,0,255,'System.Int16','<short>1</short>',8192.0,0.0,1,1,0,1,1,1,1,@TankModeDiscreteAlarmMovementTestTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Tank Mode Discrete Alarm Reverse Flow Limit' ,16,255,0,255,'System.Int16','<short>2</short>',8192.0,0.0,1,1,0,1,1,1,1,@TankModeDiscreteAlarmReverseFlowTestTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Tank Mode Discrete Alarm No Flow Limit' ,16,255,0,255,'System.Int16','<short>4</short>',8192.0,0.0,1,1,0,1,1,1,1,@TankModeDiscreteAlarmNoFlowTestTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Tank Mode Discrete Alarm Testing Limit' ,16,255,0,255,'System.Int16','<short>8</short>',8192.0,0.0,1,1,0,1,1,1,1,@TankModeDiscreteAlarmTesttingTestTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'

		-- tags for Gauge Alarm Status
		UNION ALL
		SELECT 'Gauge Alarm Scan Failure Alarm' ,16,255,0,255,'System.String',NULL,0.0,0.0,2,1,0,1,1,1,1,@GaugeScanFailureAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Gauge Alarm Input Output Failure Alarm' ,16,255,0,255,'System.String',NULL,0.0,0.0,2,1,0,1,1,1,1,@GaugeInputOutputFailureAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Gauge Alarm Device Alarm' ,16,255,0,255,'System.String',NULL,0.0,0.0,2,1,0,1,1,1,1,@GaugeDeviceAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Gauge Alarm Configuration Change Alarm' ,16,255,0,255,'System.String',NULL,0.0,0.0,2,1,0,1,1,1,1,@GaugeConfigurationChangeAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Gauge Alarm Raise Failure Alarm' ,16,255,0,255,'System.String',NULL,0.0,0.0,2,1,0,1,1,1,1,@GaugeRaiseFailureAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Gauge Alarm Lower Failure Alarm' ,16,255,0,255,'System.String',NULL,0.0,0.0,2,1,0,1,1,1,1,@GaugeLowerFailureAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Gauge Alarm Upload Failure Alarm' ,16,255,0,255,'System.String',NULL,0.0,0.0,2,1,0,1,1,1,1,@GaugeUploadFailureAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Gauge Alarm Download Failure Alarm' ,16,255,0,255,'System.String',NULL,0.0,0.0,2,1,0,1,1,1,1,@GaugeDownloadFailureAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Gauge Alarm Overfill Failure Alarm' ,16,255,0,255,'System.String',NULL,0.0,0.0,2,1,0,1,1,1,1,@GaugeOverfillAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'


		-- tags for Gauge Limits
		UNION ALL
		SELECT 'Gauge Alarm Scan Failure Limit' ,16,255,0,255,'System.UInt32','<unsignedInt>1</unsignedInt>',4294967295.0,0.0,1,1,0,1,1,1,1,@GaugeScanFailureLimitTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Gauge Alarm Input Output Failure Limit' ,16,255,0,255,'System.UInt32','<unsignedInt>2</unsignedInt>',4294967295.0,0.0,1,1,0,1,1,1,1,@GaugeInputOutputFailureLimitTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Gauge Alarm Device Limit' ,16,255,0,255,'System.UInt32','<unsignedInt>4</unsignedInt>',4294967295.0,0.0,1,1,0,1,1,1,1,@GaugeDeviceLimitTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Gauge Alarm Configuration Change Limit' ,16,255,0,255,'System.UInt32','<unsignedInt>8</unsignedInt>',4294967295.0,0.0,1,1,0,1,1,1,1,@GaugeConfigurationChangeLimitTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Gauge Alarm Raise Failure Limit' ,16,255,0,255,'System.UInt32','<unsignedInt>16</unsignedInt>',4294967295.0,0.0,1,1,0,1,1,1,1,@GaugeRaiseFailureLimitTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Gauge Alarm Lower Failure Limit' ,16,255,0,255,'System.UInt32','<unsignedInt>32</unsignedInt>',4294967295.0,0.0,1,1,0,1,1,1,1,@GaugeLowerFailureLimitTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Gauge Alarm Upload Failure Limit' ,16,255,0,255,'System.UInt32','<unsignedInt>64</unsignedInt>',4294967295.0,0.0,1,1,0,1,1,1,1,@GaugeUploadFailureLimitTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Gauge Alarm Download Failure Limit' ,16,255,0,255,'System.UInt32','<unsignedInt>128</unsignedInt>',4294967295.0,0.0,1,1,0,1,1,1,1,@GaugeDownloadFailureLimitTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Gauge Alarm Overfill Failure Limit' ,16,255,0,255,'System.UInt32','<unsignedInt>256</unsignedInt>',4294967295.0,0.0,1,1,0,1,1,1,1,@GaugeOverfillLimitTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'

		-- tags for Tank Transfer
		UNION ALL
		SELECT 'Transfer Mode' ,16,255,0,255,'FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode','<TankTransferMode>Inactive</TankTransferMode>',0.0,0.0,1,0,0,1,1,1,1,@TransferModeTemplateTagGuid,@PointTemplateGuid,@TransferModeWellKnownGuid,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Transfer Status' ,16,255,0,255,'FMBusinessObjects.DataObjects.CodedVariables.TransferStatuses','<TransferStatuses>Inactive</TransferStatuses>',0.0,0.0,2,1,0,1,1,1,1,@TransferStatusTemplateTagGuid,@PointTemplateGuid,@TransferStatusWellKnownGuid,1,1,1,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Transfer Start Level' ,3,27,2,27,'System.Double',NULL,40.0,0.0,2,1,0,1,1,1,1,@TransferLevelStartTemplateTagGuid,@PointTemplateGuid,@TransferStartLevelWellKnownGuid,1,1,1,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Transfer Start GOV' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@TransferStartGOVTemplateTagGuid ,@PointTemplateGuid,@TransferStartVolumeGOVWellKnownGuid,1,1,1,0,0,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Transfer Start NSV' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@TransferStartNSVTemplateTagGuid ,@PointTemplateGuid,@TransferStartVolumeNSVWellKnownGuid,1,1,1,0,0,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Transfer Start Volume Water' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@TransferStartVolumeWaterTemplateTagGuid ,@PointTemplateGuid,@TransferStartVolumeWaterWellKnownGuid,1,1,1,0,0,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Transfer Start Volume' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@TransferStartVolumeTemplateTagGuid ,@PointTemplateGuid,@TransferStartVolumeWellKnownGuid,1,1,1,0,0,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Transfer Target' ,3,27,2,27,'System.Double',NULL,40.0,0.0,1,0,0,1,1,1,1,@TransferTargetSetPointTemplateTagGuid,@PointTemplateGuid,@TransferTargetWellKnownGuid ,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Transferred GOV' ,5,46,2,46,'System.Double',NULL,10000.0,-10000.0,2,1,0,1,1,0,0,@TransferredGOVTemplateTagGuid ,@PointTemplateGuid,@TransferVolumeGOVWellKnownGuid,1,1,1,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Transferred NSV' ,5,46,2,46,'System.Double',NULL,10000.0,-10000.0,2,1,0,1,1,0,0,@TransferredNSVTemplateTagGuid ,@PointTemplateGuid,@TransferVolumeNSVWellKnownGuid,1,1,1,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Transferred Volume Water' ,5,46,2,46,'System.Double',NULL,10000.0,-10000.0,2,1,0,1,1,0,0,@TransferredVolumeWaterTemplateTagGuid ,@PointTemplateGuid,@TransferVolumeWaterWellKnownGuid,1,1,1,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Transferred Volume' ,5,46,2,46,'System.Double',NULL,10000.0,-10000.0,2,1,0,1,1,0,0,@TransferredVolumeTemplateTagGuid ,@PointTemplateGuid,@TransferVolumeWellKnownGuid,1,1,1,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Transfer Time Remaining' ,16,255,0,255,'System.TimeSpan',NULL,0,0,2,1,0,1,1,1,1,@TransferTimeRemainingTemplateTagGuid ,@PointTemplateGuid,@TransferTimeRemainingWellKnownGuid,1,1,1,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Transfer Time Completion' ,16,255,0,255,'System.DateTimeOffset',NULL,0,0,2,1,0,1,1,1,1,@TransferTimeCompletionTemplateTagGuid ,@PointTemplateGuid,@TransferTimeCompletionWellKnownGuid,1,1,1,0,0,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Transfer Start Time' ,16,255,0,255,'System.DateTimeOffset',NULL,0,0,2,1,0,1,1,1,1,@TransferStartTimeTagGuid ,@PointTemplateGuid,@TransferStartTimeWellKnownGuid,1,1,0,0,1,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Stop Time' ,16,255,0,255,'System.DateTimeOffset',NULL,0,0,2,1,0,1,1,1,1,@TransferStopTimeTagGuid ,@PointTemplateGuid,@TransferStopTimeWellKnownGuid,1,1,1,0,1,'2022-06-22','Administrator','2022-06-22','Administrator'
		UNION ALL
		SELECT 'Transfer Level Target' ,3,27,2,27,'System.Double',NULL,40.0,0.0,2,1,0,1,1,1,1,@TransferLevelTargetTagGuid,@PointTemplateGuid,@TransferLevelTargetWellKnownGuid ,1,1,1,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Transfer Volume Target' ,5,46,2,46,'System.Double',NULL,10000.0,-10000.0,2,1,0,1,1,0,0,@TransferVolumeTargetTagGuid ,@PointTemplateGuid,@TransferVolumeTargetWellKnownGuid,1,1,1,0,0,'2015-02-04','Administrator','2015-02-04','Administrator'

		-- tags for Tank Transfer Alarms
		UNION ALL
		SELECT 'Transfer Discrete Alarm' ,16,255,0,255,'System.Int16',NULL,255.0,0,2,1,0,1,1,1,1,@TankTransferDiscreteAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Transfer Advisory Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@TransferAdvisoryAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Transfer Target Alarm' ,15,255,2,255,'System.String',NULL,1000,0,2,1,0,1,1,1,1,@TransferTargetAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Transfer Advisory Alarm Limit' ,16,255,0,255,'System.Int16','<short>16</short>',8192.0,0.0,1,1,0,1,1,1,1,@TransferAdvisoryAlarmLimitTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'
		UNION ALL
		SELECT 'Transfer Target Alarm Limit' ,16,255,0,255,'System.Int16','<short>32</short>',8192.0,0.0,1,1,0,1,1,1,1,@TransferShutdownAlarmLimitTemplateTagGuid,@PointTemplateGuid,null,1,0,0,0,0,'2016-04-14','Administrator','2016-04-14','Administrator'

		-- tags for Available Volume / Remaining Capacity
		UNION ALL
		SELECT 'Volume Gross Observed Available' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@GrossObservedVolumeAvailableTagGuid,@PointTemplateGuid,@VolumeGrossObservedAvailableWellKnownGuid ,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Volume Gross Observed Remaining' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@GrossObservedVolumeRemainingTagGuid,@PointTemplateGuid,@VolumeGrossObservedRemainingWellKnownGuid ,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Volume Net Standard Available' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@NetStandardVolumeAvailableTagGuid,@PointTemplateGuid,@VolumeNetStandardAvailableWellKnownGuid ,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Volume Net Standard Remaining' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@NetStandardVolumeRemainingTagGuid,@PointTemplateGuid,@VolumeNetStandardRemainingWellKnownGuid ,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'

		-- Level Product Rate Alarm Tags
		UNION ALL
		SELECT 'Level Product Rate Alarm Emptying', 15, 255, 2 ,255, 'System.String', NULL, 1, 0, 2, 1, 0, 1, 1, 1, 1, @LevelProductRateAlarmEmptyingTemplateTagGuid, @PointTemplateGuid, null, 1, 1, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Level Product Rate Alarm Filling', 15, 255, 2 ,255, 'System.String', NULL, 1, 0, 2, 1, 0, 1, 1, 1, 1, @LevelProductRateAlarmFillingTemplateTagGuid, @PointTemplateGuid, null, 1, 1, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Level Product Rate Limit Emptying', 10, 162, 2 ,162, 'System.Double', '<double>-9.5</double>', 0, -10, 1, 1, 0, 1, 1, 0, 1, @LevelProductRateAlarmLimitEmptyingTemplateTagGuid, @PointTemplateGuid, null, 1, 0, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Level Product Rate Limit Filling', 10, 162, 2 ,162, 'System.Double', '<double>9.5</double>', 10, 0, 1, 1, 0, 1, 1, 1, 0, @LevelProductRateAlarmLimitFillingTemplateTagGuid ,@PointTemplateGuid, null, 1, 0, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'

		-- Flow Rate TOV (Strap Volume) Alarm Tags
		UNION ALL
		SELECT 'Volume Total Observed Rate Alarm Emptying', 15, 255, 2 ,255, 'System.String', NULL, 0, -1000, 2, 1, 0, 1, 1, 1, 1, @VolumeTotalObservedRateAlarmEmptyingTemplateTagGuid, @PointTemplateGuid, null, 1, 1, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Volume Total Observed Rate Alarm Filling', 15, 255, 2 ,255, 'System.String', NULL, 1000, 0, 2, 1, 0, 1, 1, 1, 1, @VolumeTotalObservedRateAlarmFillingTemplateTagGuid, @PointTemplateGuid, null, 1, 1, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Volume Total Observed Rate Limit Emptying', 8, 109, 2 ,109, 'System.Double', '<double>-950</double>', 0, -1000, 1, 1, 0, 1, 1, 0, 1, @VolumeTotalObservedRateAlarmLimitEmptyingTemplateTagGuid, @PointTemplateGuid, null, 1, 0, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Volume Total Observed Rate Limit Filling', 8, 109, 2 ,109, 'System.Double', '<double>950</double>', 1000, 0, 1, 1, 0, 1, 1, 1, 0, @VolumeTotalObservedRateAlarmLimitFillingTemplateTagGuid ,@PointTemplateGuid, null, 1, 0, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'

		-- Flow Rate NSV Alarm Tags
		UNION ALL
		SELECT 'Volume Net Standard Rate Alarm Emptying', 15, 255, 2 ,255, 'System.String', NULL, 0, -1000, 2, 1, 0, 1, 1, 1, 1, @VolumeNetStandardRateAlarmEmptyingTemplateTagGuid, @PointTemplateGuid, null, 1, 1, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Volume Net Standard Rate Alarm Filling', 15, 255, 2 ,255, 'System.String', NULL, 1000, 0, 2, 1, 0, 1, 1, 1, 1, @VolumeNetStandardRateAlarmFillingTemplateTagGuid, @PointTemplateGuid, null, 1, 1, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Volume Net Standard Rate Limit Emptying', 8, 109, 2 ,109, 'System.Double', '<double>-950</double>', 0, -1000, 1, 1, 0, 1, 1, 0, 1, @VolumeNetStandardRateAlarmLimitEmptyingTemplateTagGuid, @PointTemplateGuid, null, 1, 0, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Volume Net Standard Rate Limit Filling', 8, 109, 2 ,109, 'System.Double', '<double>950</double>', 1000, 0, 1, 1, 0, 1, 1, 1, 0, @VolumeNetStandardRateAlarmLimitFillingTemplateTagGuid ,@PointTemplateGuid, null, 1, 0, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'

		-- Flow Rate GOV Alarm Tags
		UNION ALL
		SELECT 'Volume Gross Observed Rate Alarm Emptying', 15, 255, 2 ,255, 'System.String', NULL, 0, -1000, 2, 1, 0, 1, 1, 1, 1, @VolumeGrossObservedRateAlarmEmptyingTemplateTagGuid, @PointTemplateGuid, null, 1, 1, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Volume Gross Observed Rate Alarm Filling', 15, 255, 2 ,255, 'System.String', NULL, 1000, 0, 2, 1, 0, 1, 1, 1, 1, @VolumeGrossObservedRateAlarmFillingTemplateTagGuid, @PointTemplateGuid, null, 1, 1, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Volume Gross Observed Rate Limit Emptying', 8, 109, 2 ,109, 'System.Double', '<double>-950</double>', 0, -1000, 1, 1, 0, 1, 1, 0, 1, @VolumeGrossObservedRateAlarmLimitEmptyingTemplateTagGuid, @PointTemplateGuid, null, 1, 0, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'
		UNION ALL
		SELECT 'Volume Gross Observed Rate Limit Filling', 8, 109, 2 ,109, 'System.Double', '<double>950</double>', 1000, 0, 1, 1, 0, 1, 1, 1, 0, @VolumeGrossObservedRateAlarmLimitFillingTemplateTagGuid ,@PointTemplateGuid, null, 1, 0, 0, 0, 0,'2017-09-07', 'Administrator', '2017-09-07', 'Administrator'

		-- Vapor Tags
		UNION ALL
		SELECT 'Density Vapor' ,11,191,2,191,'System.Double',NULL,100.0,0,1,1,0,1,1,1,1,@DensityVaporTemplateTagGuid ,@PointTemplateGuid,null,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Volume Vapor Net' ,5,46,2,46,'System.Double',NULL,10000.0,0,2,1,0,1,1,1,1,@VolumeVaporNetTemplateTagGuid ,@PointTemplateGuid,null,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT 'Mass Vapor' ,6,64,2,64,'System.Double',NULL,10000000.0,0,2,1,0,1,1,1,1,@MassVaporTemplateTagGuid ,@PointTemplateGuid,null,1,0,0,0,1,'2015-02-04','Administrator','2015-02-04','Administrator'

		-- tags for Miscelaneous
		UNION ALL
		SELECT 'Operational Mode' ,16,255,0,255,'FMBusinessObjects.DataObjects.CodedVariables.TankOperationalMode','<TankOperationalMode>Normal</TankOperationalMode>',0.0,0.0,1,1,0,1,1,1,1,@OperationalModeTemplateTagGuid,@PointTemplateGuid,@OperationalModeWellKnownGuid,0,0,0,0,0,'2020-09-21','Administrator','2020-09-21','Administrator'

		-- Leak Detection Tags
		UNION ALL
		SELECT 'Leak Detection Alarm',16,255,0,255,'System.String',NULL,1,0,2,1,0,1,1,1,1,@LeakDetectionAlarmTagGuid,@PointTemplateGuid,NULL,1,1,0,1,0,'2023-04-05','Administrator','2023-04-05','Administrator'
		UNION ALL
		SELECT 'Leak Detection Data Insufficient Limit',16,255,0,255,'System.Int16','<short>1</short>',1,0,1,1,0,1,1,1,1,@LeakDetectionDataInsufficientLimitTagGuid,@PointTemplateGuid,NULL,1,1,0,1,0,'2023-04-05','Administrator','2023-04-05','Administrator'
		UNION ALL
		SELECT 'Leak Detection Discrete Alarm',16,255,0,255,'System.Int16',NULL,1,0,5,1,0,1,1,1,1,@LeakDetectionDiscreteAlarmTagGuid ,@PointTemplateGuid,NULL,1,1,0,1,1,'2023-04-05','Administrator','2023-04-05','Administrator'
		UNION ALL
		SELECT 'Leak Rate',8,110,2,110,'System.Double',NULL,10.0,-10.0,5,1,0,0,0,0,0,@LeakRateTagGuid,@PointTemplateGuid,@LeakRateWellKnownGuid,1,0,0,1,1,'2023-04-05','Administrator','2023-04-05','Administrator'
		UNION ALL
		SELECT 'Leak Rate High Alarm',16,255,0,255,'System.String',NULL,100,0,2,1,0,1,1,1,1,@LeakRateHighAlarmTagGuid,@PointTemplateGuid,NULL,1,0,0,1,0,'2023-04-05','Administrator','2023-04-05','Administrator'
		UNION ALL
		SELECT 'Leak Rate High Limit',8,110,2,110,'System.Double','<double>0.2</double>',10.0,-10.0,1,1,0,0,0,0,0,@LeakRateHighLimitTagGuid,@PointTemplateGuid,NULL,1,0,0,1,0,'2023-04-05','Administrator','2023-04-05','Administrator'
		UNION ALL
		SELECT 'Pressure Bottom',7,73,2,73,'System.Double',NULL,30,0,3,1,0,1,1,1,1,@PressureBottomTagGuid,@PointTemplateGuid,@PressureBottomWellKnownGuid,1,0,0,1,1,'2023-04-05','Administrator','2023-04-05','Administrator'
		UNION ALL
		SELECT 'Volume Correction Factor Unrounded',15,255,9,255,'System.Double',NULL,2,0,2,1,0,1,0,0,0,@VolumeCorrectionFactorUnroundedTagGuid,@PointTemplateGuid,@VolumeCorrectionFactorUnroundedWellKnownGuid,1,0,0,1,1,'2023-04-05','Administrator','2023-04-05','Administrator'
		UNION ALL
		SELECT 'Volume Net Standard Unrounded',5,46,6,46,'System.Double',NULL,10000,0,2,1,0,1,0,0,0,@VolumeNetStandardUnroundedTagGuid,@PointTemplateGuid,@VolumeNetStandardUnroundedWellKnownGuid,1,0,0,1,1,'2023-04-05','Administrator','2023-04-05','Administrator'
		UNION ALL
		SELECT 'Leak Detection Data Last Run Time',16,255,0,255,'System.DateTimeOffset',NULL,0,0,5,1,0,1,1,1,1,@LeakDetectionDataLastRunTimeTagGuid,@PointTemplateGuid,@LeakDetectionDataLastRunTimeWellKnownGuid,0,1,0,1,1,'2023-04-05','Administrator','2023-04-05','Administrator'

		-- Tags for TAC Fuels
		UNION ALL
		SELECT 'Number Strap Points',16,255,0,255,'System.Int16',NULL,1000,0,1,1,1,0,0,0,0,@NumberStrapPointTagGuid,@PointTemplateGuid,null,0,0,0,0,0,'2023-10-31','Administrator','2023-10-31','Administrator'
		UNION ALL
		SELECT 'Strap Table Select',16,255,0,255,'FMBusinessObjects.DataObjects.PointCommandStatusListReference',@StandardTankStrapTableSelectCommandStatusValue,1,0,1,0,0,1,1,1,1,@StrapTableSelectTagGuid,@PointTemplateGuid,null,0,0,0,0,0,'2023-10-31','Administrator','2023-10-31','Administrator'
		UNION ALL
		SELECT 'Pressure Gauge Filter',16,255,6,255,'System.Double',NULL,1,0.01,1,0,0,1,1,0,0,@PressureGaugeFilterTagGuid,@PointTemplateGuid,null,0,0,0,0,0,'2023-10-31','Administrator','2023-10-31','Administrator'

		-- Volume Gross Observed Alarm Tags
		UNION ALL
		SELECT 'Volume Gross Observed Low Limit' ,5,46,2,46,'System.Double','<double>1000.0</double>',10000.0,0.0,1,1,0,1,1,1,1,@VolumeGrossObservedLowLimitTemplateTagGuid,@PointTemplateGuid,@VolumeGrossObservedLowWellKnownGuid,1,0,0,0,0,'2023-11-21','Administrator','2023-11-21','Administrator'
		UNION ALL
		SELECT 'Volume Gross Observed LoLo Limit' ,5,46,2,46,'System.Double','<double>500.0</double>',10000.0,0.0,1,1,0,1,1,1,1,@VolumeGrossObservedLoLoLimitTemplateTagGuid,@PointTemplateGuid,@VolumeGrossObservedLoLoWellKnownGuid,1,0,0,0,0,'2023-11-21','Administrator','2023-11-21','Administrator'
		UNION ALL
		SELECT 'Volume Gross Observed High Limit' ,5,46,2,46,'System.Double','<double>9000.0</double>',10000.0,0.0,1,1,0,1,1,1,1,@VolumeGrossObservedHighLimitTemplateTagGuid,@PointTemplateGuid,@VolumeGrossObservedHighWellKnownGuid,1,0,0,0,0,'2023-11-21','Administrator','2023-11-21','Administrator'
		UNION ALL
		SELECT 'Volume Gross Observed HiHi Limit' ,5,46,2,46,'System.Double','<double>9500.0</double>',10000.0,0.0,1,1,0,1,1,1,1,@VolumeGrossObservedHiHiLimitTemplateTagGuid,@PointTemplateGuid,@VolumeGrossObservedHiHiWellKnownGuid,1,0,0,0,0,'2023-11-21','Administrator','2023-11-21','Administrator'
		UNION ALL
		SELECT 'Volume Gross Observed HiHi Alarm' ,15,255,2,255,'System.String',NULL,10000,0,2,1,0,1,1,1,1,@VolumeGrossObservedHiHiAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2023-11-21','Administrator','2023-11-21','Administrator'
		UNION ALL
		SELECT 'Volume Gross Observed High Alarm' ,15,255,2,255,'System.String',NULL,10000,0,2,1,0,1,1,1,1,@VolumeGrossObservedHighAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2023-11-21','Administrator','2023-11-21','Administrator'
		UNION ALL
		SELECT 'Volume Gross Observed Low Alarm' ,15,255,2,255,'System.String',NULL,10000,0,2,1,0,1,1,1,1,@VolumeGrossObservedLowAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2023-11-21','Administrator','2023-11-21','Administrator'
		UNION ALL
		SELECT 'Volume Gross Observed LoLo Alarm' ,15,255,2,255,'System.String',NULL,10000,0,2,1,0,1,1,1,1,@VolumeGrossObservedLoLoAlarmTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2023-11-21','Administrator','2023-11-21','Administrator'
		
		-- Enable Disable tank command
		UNION ALL
		SELECT 'Enable Disable' ,16,255,0,255,'FMBusinessObjects.DataObjects.PointCommandStatusListReference',@StandardTankEnableDisableCommandStatusValue,9999.0,0.0,1,0,0,1,1,1,1,@EnableDisableCommandTemplateTagGuid,@PointTemplateGuid,null,1,1,0,0,0,'2023-12-05','Administrator','2023-12-05','Administrator'

) 
AS Source
ON (Target.[PointTemplateGuid] = Source.[PointTemplateGuid] AND Target.PointTemplateTagGuid = Source.PointTemplateTagGuid)
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
						target.[EngineeringUnitsType] = source.[EngineeringUnitsType],
						target.[DecimalPlaces] = source.[DecimalPlaces],
						target.[ServerEngineeringUnitsIndex] = source.[ServerEngineeringUnitsIndex],
						target.[ValueType] = source.[ValueType],
						target.[Value] = (CASE
							WHEN source.[ValueType] = 'FMBusinessObjects.DataObjects.PointCommandStatusListReference' THEN source.[Value]
							ELSE target.[Value]
							END),
						target.[PointTagInputOutputTypeIndex] = source.[PointTagInputOutputTypeIndex],  -- enum PointTagInputOutputType {UnAssigned = 0, Manual = 1, Calculated = 2, OpcUa = 3, FCEE = 4}
						target.[Input] = source.[Input],
						target.[AlarmStatus] = source.[AlarmStatus],
						target.[ApplyPointTemplateEngineeringUnits] = source.[ApplyPointTemplateEngineeringUnits],
						target.[ApplyPointTemplateDecimalPlaces] = source.[ApplyPointTemplateDecimalPlaces],
						target.[ApplyPointTemplateMaximum] = source.[ApplyPointTemplateMaximum],
						target.[ApplyPointTemplateMinimum] = source.[ApplyPointTemplateMinimum],
						target.[PointTemplateTagGuid] = source.[PointTemplateTagGuid],
						target.[PointTemplateGuid] = source.[PointTemplateGuid],
						target.[WellKnownIdentityGuid] = source.[WellKnownIdentityGuid],
						target.[InhibitInputOutputTypeConfiguration] = source.[InhibitInputOutputTypeConfiguration],
						target.[InhibitOverride] = source.[InhibitOverride],
						target.[Module] = source.[Module],
						target.[Archived] = source.[Archived],
						target.[UpdatedDate] =  SYSDATETIMEOFFSET(),
						target.[UpdatedBy] = source.[UpdatedBy]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[EngineeringUnitsType],[EngineeringUnitsIndex],[DecimalPlaces],[ServerEngineeringUnitsIndex],[ValueType],
		[Value],[Maximum],[Minimum],[PointTagInputOutputTypeIndex],[Input],[AlarmStatus],[ApplyPointTemplateEngineeringUnits],
		[ApplyPointTemplateDecimalPlaces],[ApplyPointTemplateMaximum],[ApplyPointTemplateMinimum],[PointTemplateTagGuid],
		[PointTemplateGuid],[WellKnownIdentityGuid],[AlarmsEnabled],[InhibitInputOutputTypeConfiguration],[InhibitOverride],[Module],[Archived],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
	VALUES (Source.[ID],Source.[EngineeringUnitsType],Source.[EngineeringUnitsIndex],Source.[DecimalPlaces],Source.[ServerEngineeringUnitsIndex],Source.[ValueType],
		Source.[Value],Source.[Maximum],Source.[Minimum],Source.[PointTagInputOutputTypeIndex],Source.[Input],Source.[AlarmStatus],Source.[ApplyPointTemplateEngineeringUnits],
		Source.[ApplyPointTemplateDecimalPlaces],Source.[ApplyPointTemplateMaximum],Source.[ApplyPointTemplateMinimum],Source.[PointTemplateTagGuid],
		Source.[PointTemplateGuid],Source.[WellKnownIdentityGuid],Source.[AlarmsEnabled],Source.[InhibitInputOutputTypeConfiguration],Source.[InhibitOverride],Source.[Module],Source.[Archived],Source.[CreatedDate],Source.[CreatedBy],Source.[UpdatedDate],Source.[UpdatedBy]);

-- New Alarms are created in this Section
DECLARE @AlarmPointTemplateTypeIndex INT = (SELECT ApplicationStringTypeIndex FROM lookup.tblApplicationStringType WHERE ApplicationStringTypeCode = 'ALARM_EVENT_CATEGORY')
DECLARE @AlarmApplicationStringGuid UNIQUEIDENTIFIER = '512ab266-b3b8-4a29-b8d9-594795cf63ed'
DECLARE @AlarmPointSiteGuid UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001'
DECLARE @AlarmApplicationString NVARCHAR(30) = 'Alarm Group'


DECLARE @HiHiLoLoAlarmPriorityGuid UNIQUEIDENTIFIER = 'aa9e557c-a652-4caf-9bca-2bcb9ab5b104'
DECLARE @HighLowAlarmPriorityGuid UNIQUEIDENTIFIER = 'BA35E686-5CCE-402D-982B-18D45958CCB6'
DECLARE @MaxMinOperatingAlarmPriorityGuid UNIQUEIDENTIFIER = '402A7722-062B-42F6-B6A5-E6180E2BA2B8'
DECLARE @NormalUnacknowledgedAlarmPriorityGuid UNIQUEIDENTIFIER = '5B7D7344-7D3C-4CDE-A834-B5E2C8BFE11F'

IF ((SELECT COUNT(ID) FROM tblApplicationString WHERE ID = @AlarmApplicationString) = 0)
BEGIN
	INSERT INTO tblApplicationString (ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
	VALUES (@AlarmApplicationString, '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @AlarmApplicationStringGuid, @AlarmPointSiteGuid, @AlarmPointTemplateTypeIndex)

	INSERT INTO [map].[tblEntityAlarmAndEventCategoryToSite]([AlarmAndEventCategoryToSiteGuid],[ApplicationStringGuid],[SiteGuid],[CreatedDate],[CreatedBy],[UpdatedDate] ,[UpdatedBy],[AssignedFromSiteGuid])
	VALUES ('4af3ac4c-d1c3-40f5-84ba-ad804be01142',@AlarmApplicationStringGuid,@AlarmPointSiteGuid,'2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator',@AlarmPointSiteGuid)
END

IF ((SELECT COUNT(AlarmPriorityGuid) FROM tblAlarmPriorities WHERE AlarmPriorityGuid = @HiHiLoLoAlarmPriorityGuid) = 0)
BEGIN
	INSERT INTO [dbo].[tblAlarmPriorities] ([ID],[BackgroundSteady],[BackgroundAlternate],[TextSteady],[TextAlternate],[SoundFile],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AlarmPriorityGuid],[SiteGuid],[Priority])
	VALUES ('HiHi/LoLo', 'FF0000', '000000', '000000', 'FF0000', 'fmsound01.mp3', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @HiHiLoLoAlarmPriorityGuid, '00000000-0000-0000-0000-000000000001', 1)

	INSERT INTO [map].[tblEntityAlarmPriorityToSite] ([AlarmPriorityToSiteGuid],[AlarmPriorityGuid],[SiteGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AssignedFromSiteGuid])
	VALUES('40243893-176c-4800-b71f-989801c6dd3a', @HiHiLoLoAlarmPriorityGuid, '00000000-0000-0000-0000-000000000001', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '00000000-0000-0000-0000-000000000001')
END

IF ((SELECT COUNT(AlarmPriorityGuid) FROM tblAlarmPriorities WHERE AlarmPriorityGuid = @HighLowAlarmPriorityGuid) = 0)
BEGIN
	INSERT INTO [dbo].[tblAlarmPriorities] ([ID],[BackgroundSteady],[BackgroundAlternate],[TextSteady],[TextAlternate],[SoundFile],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AlarmPriorityGuid],[SiteGuid],[Priority])
	VALUES ('High/Low', 'FFFF00', '000000', '000000', 'FFFF00', 'fmsound02.mp3', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @HighLowAlarmPriorityGuid, '00000000-0000-0000-0000-000000000001', 2)

	INSERT INTO [map].[tblEntityAlarmPriorityToSite] ([AlarmPriorityToSiteGuid],[AlarmPriorityGuid],[SiteGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AssignedFromSiteGuid])
	VALUES('523BA5D3-2FA0-425C-94D8-726155545ABD', @HighLowAlarmPriorityGuid, '00000000-0000-0000-0000-000000000001', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '00000000-0000-0000-0000-000000000001')
END

IF ((SELECT COUNT(AlarmPriorityGuid) FROM tblAlarmPriorities WHERE AlarmPriorityGuid = @MaxMinOperatingAlarmPriorityGuid) = 0)
BEGIN
	INSERT INTO [dbo].[tblAlarmPriorities] ([ID],[BackgroundSteady],[BackgroundAlternate],[TextSteady],[TextAlternate],[SoundFile],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AlarmPriorityGuid],[SiteGuid],[Priority])
	VALUES ('Min/Max Operating', 'FF00FF', '000000', '000000', 'FF00FF', 'fmsound00.mp3', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @MaxMinOperatingAlarmPriorityGuid, '00000000-0000-0000-0000-000000000001', 3)

	INSERT INTO [map].[tblEntityAlarmPriorityToSite] ([AlarmPriorityToSiteGuid],[AlarmPriorityGuid],[SiteGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AssignedFromSiteGuid])
	VALUES('34F4ED7B-4C0E-414D-A513-919630B04C97', @MaxMinOperatingAlarmPriorityGuid, '00000000-0000-0000-0000-000000000001', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '00000000-0000-0000-0000-000000000001')
END

-- Alarm Template
DECLARE @LevelHiHiAlarmTemplateGuid UNIQUEIDENTIFIER = '54635f19-7bfe-4342-80f9-9fcf6e1e3fbc'
DECLARE @LevelHighAlarmTemplateGuid UNIQUEIDENTIFIER = 'b9188b9f-a233-4b19-af97-2ed9614c6915'
DECLARE @LevelMaxOpAlarmTemplateGuid UNIQUEIDENTIFIER = '6cd6f5f3-a00b-416b-ba01-7b05eb0b5767'
DECLARE @LevelMinOpAlarmTemplateGuid UNIQUEIDENTIFIER = 'eafffa36-0a10-48dd-b321-c6ef1eb3cec7'
DECLARE @LevelLowAlarmTemplateGuid UNIQUEIDENTIFIER = 'cd5a8fc5-345c-4aa5-84d8-c6feadf74024'
DECLARE @LevelLoLoAlarmTemplateGuid UNIQUEIDENTIFIER = 'd3e18ff1-8227-4e8c-a166-45cb3515b887'


DECLARE @TemperatureHiHiAlarmTemplateGuid UNIQUEIDENTIFIER = 'D07868EF-A757-4DC9-9527-1EF3C7AEFE83'
DECLARE @TemperatureHighAlarmTemplateGuid UNIQUEIDENTIFIER = '4B1F7032-18BE-4F15-B1DC-875A4B28C12E'
DECLARE @TemperatureLowAlarmTemplateGuid UNIQUEIDENTIFIER = 'EAE9B782-0347-4DDA-9415-AE5AA60325E4'
DECLARE @TemperatureLoLoAlarmTemplateGuid UNIQUEIDENTIFIER = 'A624F071-994F-41DA-8D6F-F064B08FE494'

DECLARE @VolumeGrossObservedHiHiAlarmTemplateGuid UNIQUEIDENTIFIER = 'AB11DFAF-17CC-4D26-AF7E-6D390DD7321D'
DECLARE @VolumeGrossObservedHighAlarmTemplateGuid UNIQUEIDENTIFIER = '7404D525-F948-4731-B318-D8F6A00E80BC'
DECLARE @VolumeGrossObservedLowAlarmTemplateGuid UNIQUEIDENTIFIER = 'EA56726A-7B00-4228-8616-98AD0FC22164'
DECLARE @VolumeGrossObservedLoLoAlarmTemplateGuid UNIQUEIDENTIFIER = '4844009C-3B7A-46D5-B208-A1633BC2EBF4'

DECLARE @DensityHighAlarmTemplateGuid UNIQUEIDENTIFIER = '5A2322A0-F0CF-489D-BB0F-79F71D3B08D1'
DECLARE @DensityLowAlarmTemplateGuid UNIQUEIDENTIFIER = '5115726C-38C3-41AE-9D85-0A5F09EA2B8E'

DECLARE @TankModeDiscreteAlarmGuid UNIQUEIDENTIFIER = '1E4A3664-2EC0-4D68-9868-6909E8CB653C'
DECLARE @TankTransferTargetDiscreteAlarmGuid UNIQUEIDENTIFIER = 'FA91AF38-3E04-4D08-859A-A109C83246A2'
DECLARE @TankTransferAdvisoryDiscreteAlarmGuid UNIQUEIDENTIFIER = 'AAB475A5-AAA5-4745-A8E2-6A37ED143018'

-- Level Product Rate Alarm template GUID
DECLARE @LevelProductRateAlarmEmptyingTemplateGuid		UNIQUEIDENTIFIER = '1FDCD009-1C39-4F88-95C0-26226BB75E83'
DECLARE @LevelProductRateAlarmFillingTemplateGuid		UNIQUEIDENTIFIER = '0A900400-046E-44F1-A5B4-85179144CB77'

-- Flow TOV Rate Alarm template GUID
DECLARE @VolumeTotalObservedRateAlarmEmptyingTemplateGuid		UNIQUEIDENTIFIER = 'FC0C3A15-9BE6-4DB3-92FD-0EC5B252DE1B'
DECLARE @VolumeTotalObservedRateAlarmFillingTemplateGuid		UNIQUEIDENTIFIER = '1D2244FC-46BA-433E-A765-26CFE304B822'

-- Flow NSV Rate Alarm template GUID
DECLARE @VolumeNetStandardRateAlarmEmptyingTemplateGuid			UNIQUEIDENTIFIER = '1CCEA593-E915-4A33-AA29-FEA6D10C84FE'
DECLARE @VolumeNetStandardRateAlarmFillingTemplateGuid			UNIQUEIDENTIFIER = '9CC6235F-D98A-47CD-B0EF-9BF6F45C0DFE'

-- Flow GOV Rate Alarm template GUID
DECLARE @VolumeGrossObservedRateAlarmEmptyingTemplateGuid		UNIQUEIDENTIFIER = 'D90D8309-6F48-405D-B5E4-91072BFE820A'
DECLARE @VolumeGrossObservedRateAlarmFillingTemplateGuid		UNIQUEIDENTIFIER = 'FFE33427-A29A-4F99-A209-1390C5753601'

-- Gauge Alarm
DECLARE @GaugeScanFailureAlarmGuid UNIQUEIDENTIFIER = '99529D5E-61F9-4897-8D9E-902298053236'
DECLARE @GaugeInputOutputFailureAlarmGuid UNIQUEIDENTIFIER = '2B5E2875-F269-4D48-AC9E-E6FEE825423F'
DECLARE @GaugeDeviceAlarmGuid UNIQUEIDENTIFIER = '05A6E8A3-1BD1-4B41-AEEF-466FB0BA50A2'
DECLARE @GaugeConfigurationChangeAlarmGuid UNIQUEIDENTIFIER = 'F07857A5-49C8-4D86-AE1C-101FD4BF0CB8'
DECLARE @GaugeRaiseFailureAlarmGuid UNIQUEIDENTIFIER = '003A3AB9-9544-4276-9C21-BCC90B150915'
DECLARE @GaugeLowerFailureAlarmGuid UNIQUEIDENTIFIER = 'DA812341-5E1E-4A3B-956C-BB50E028B6F0'
DECLARE @GaugeUploadFailureAlarmGuid UNIQUEIDENTIFIER = '869A7CE7-D822-4143-B2E5-D2F03DA34170'
DECLARE @GaugeDownloadFailureAlarmGuid UNIQUEIDENTIFIER = '8B62C2B6-A16A-4616-85AD-DF366040CF8C'
DECLARE @GaugeOverfillAlarmGuid UNIQUEIDENTIFIER = '795DC665-33DC-4E3A-95DF-C94F1399150F'

-- Leak Detection Alarm Template
DECLARE @LeakDetectionHighAlarmTemplateGuid UNIQUEIDENTIFIER = '0610A865-968B-436D-9CEA-B35E6A44AF78'
DECLARE @LeakDetectionDataInsufficientAlarmTemplateGuid UNIQUEIDENTIFIER = 'CEFF72E7-6DC3-4721-AA53-70B5794CEBB6'

MERGE dbo.tblAlarmTemplate AS Target
USING 
(  SELECT @LevelHiHiAlarmTemplateGuid AS [AlarmTemplateGuid]
			,@LevelTemplateTagGuid AS [InputTemplateTagGuid]
			,'Level HiHi Alarm' AS [ID]
			,1 AS [Enabled]
			,@AlarmApplicationStringGuid AS [AlarmCategoryApplicationStringGuid]
			,0 AS [Order]
			,'Normal' AS [NotAlarmState]
			,'Alarm Comment' AS [Comment]
			,null AS [ShelvedStartTimeStamp]
			,null AS [ShelvedEndTimeStamp]
			,0 AS [ShelvedOneShot]
			,0 AS [Suppressed]
			,'2015-02-04' as [CreatedDate]
			,'Administrator' as [CreatedBy]
			,'2015-02-04' as [UpdatedDate]
			,'Administrator' as [UpdatedBy]
			,@LevelHiHiAlarmTemplateTagGuid AS [AlarmStateTemplateTagGuid]
			,1 AS [ExclusiveAlarm]
	UNION ALL
	SELECT @LevelHighAlarmTemplateGuid,@LevelTemplateTagGuid,'Level High Alarm',1,@AlarmApplicationStringGuid,1,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@LevelHighAlarmTemplateTagGuid,1
	UNION ALL
	SELECT @LevelMaxOpAlarmTemplateGuid,@LevelTemplateTagGuid,'Level Max Op Alarm',1,@AlarmApplicationStringGuid,2,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@LevelMaxOpAlarmTemplateTagGuid,1
	UNION ALL
	SELECT @LevelMinOpAlarmTemplateGuid,@LevelTemplateTagGuid,'Level Min Op Alarm',1,@AlarmApplicationStringGuid,2,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@LevelMinOpAlarmTemplateTagGuid,1
	UNION ALL
	SELECT @LevelLowAlarmTemplateGuid,@LevelTemplateTagGuid,'Level Low Alarm',1,@AlarmApplicationStringGuid,1,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@LevelLowAlarmTemplateTagGuid,1
	UNION ALL
	SELECT @LevelLoLOAlarmTemplateGuid,@LevelTemplateTagGuid,'Level LoLo Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@LevelLoLoAlarmTemplateTagGuid,1
	UNION ALL
	SELECT @TemperatureHiHiAlarmTemplateGuid,@TemperatureProductTemplateTagGuid,'Temperature HiHi Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@TemperatureHiHiAlarmTemplateTagGuid,1
	UNION ALL
	SELECT @TemperatureHighAlarmTemplateGuid,@TemperatureProductTemplateTagGuid,'Temperature High Alarm',1,@AlarmApplicationStringGuid,1,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@TemperatureHighAlarmTemplateTagGuid,1
	UNION ALL
	SELECT @TemperatureLowAlarmTemplateGuid,@TemperatureProductTemplateTagGuid,'Temperature Low Alarm',1,@AlarmApplicationStringGuid,1,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@TemperatureLowAlarmTemplateTagGuid,1
	UNION ALL
	SELECT @TemperatureLoLoAlarmTemplateGuid,@TemperatureProductTemplateTagGuid,'Temperature LoLo Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@TemperatureLoLoAlarmTemplateTagGuid,1
	UNION ALL
	SELECT @DensityHighAlarmTemplateGuid,@DensityProductObservedTemplateTagGuid,'Density High Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@DensityHighAlarmTemplateTagGuid,1
	UNION ALL
	SELECT @DensityLowAlarmTemplateGuid,@DensityProductObservedTemplateTagGuid,'Density Low Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@DensityLowAlarmTemplateTagGuid,1
	UNION ALL
	SELECT @TankModeDiscreteAlarmGuid,@TankModeDiscreteAlarmTagGuid,'Tank Mode Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@TankModeAlarmTagGuid,1
	UNION ALL
	SELECT @TankTransferTargetDiscreteAlarmGuid,@TankTransferDiscreteAlarmTemplateTagGuid,'Tank Transfer Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@TransferTargetAlarmTemplateTagGuid,1
	UNION ALL
	SELECT @TankTransferAdvisoryDiscreteAlarmGuid,@TankTransferDiscreteAlarmTemplateTagGuid,'Tank Transfer Advisory Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2015-02-04','Administrator','2015-02-04','Administrator',@TransferAdvisoryAlarmTemplateTagGuid,1

	-- Level Product Rate Alarm template tags
	UNION ALL
	SELECT @LevelProductRateAlarmEmptyingTemplateGuid, @LevelProductRateTagGuid, 'Level Product Rate Alarm Emptying', 1, @AlarmApplicationStringGuid, 0, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @LevelProductRateAlarmEmptyingTemplateTagGuid, 1
	UNION ALL
	SELECT @LevelProductRateAlarmFillingTemplateGuid, @LevelProductRateTagGuid, 'Level Product Rate Alarm Filling', 1, @AlarmApplicationStringGuid, 0, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @LevelProductRateAlarmFillingTemplateTagGuid, 1

	-- Flow TOV Rate Alarm template tags
	UNION ALL
	SELECT @VolumeTotalObservedRateAlarmEmptyingTemplateGuid, @VolumeTotalObservedRateTagGuid, 'Volume Total Observed Rate Alarm Emptying', 1, @AlarmApplicationStringGuid, 0, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @VolumeTotalObservedRateAlarmEmptyingTemplateTagGuid, 1
	UNION ALL
	SELECT @VolumeTotalObservedRateAlarmFillingTemplateGuid, @VolumeTotalObservedRateTagGuid, 'Volume Total Observed Rate Alarm Filling', 1, @AlarmApplicationStringGuid, 0, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @VolumeTotalObservedRateAlarmFillingTemplateTagGuid, 1

	-- Flow NSV Rate Alarm template tags
	UNION ALL
	SELECT @VolumeNetStandardRateAlarmEmptyingTemplateGuid, @VolumeNetStandardRateTagGuid, 'Volume Net Standard Rate Alarm Emptying', 1, @AlarmApplicationStringGuid, 0, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @VolumeNetStandardRateAlarmEmptyingTemplateTagGuid, 1
	UNION ALL
	SELECT @VolumeNetStandardRateAlarmFillingTemplateGuid, @VolumeNetStandardRateTagGuid, 'Volume Net Standard Rate Alarm Filling', 1, @AlarmApplicationStringGuid, 0, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @VolumeNetStandardRateAlarmFillingTemplateTagGuid, 1

	-- Flow GOV Rate Alarm template tags
	UNION ALL
	SELECT @VolumeGrossObservedRateAlarmEmptyingTemplateGuid, @VolumeGrossObservedRateTagGuid, 'Volume Gross Observed Rate Alarm Emptying', 1, @AlarmApplicationStringGuid, 0, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @VolumeGrossObservedRateAlarmEmptyingTemplateTagGuid, 1
	UNION ALL
	SELECT @VolumeGrossObservedRateAlarmFillingTemplateGuid, @VolumeGrossObservedRateTagGuid, 'Volume Gross Observed Rate Alarm Filling', 1, @AlarmApplicationStringGuid, 0, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @VolumeGrossObservedRateAlarmFillingTemplateTagGuid, 1

	-- Gauge Alarm
	UNION ALL
	SELECT @GaugeScanFailureAlarmGuid, @GaugeAlarm8130TemplateTagGuid, 'Scan Failure Alarm', 1, @AlarmApplicationStringGuid, 8, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @GaugeScanFailureAlarmTemplateTagGuid, 1
	UNION ALL
	SELECT @GaugeInputOutputFailureAlarmGuid, @GaugeAlarm8130TemplateTagGuid, 'Input Output Failure Alarm', 1, @AlarmApplicationStringGuid, 7, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @GaugeInputOutputFailureAlarmTemplateTagGuid, 1
	UNION ALL
	SELECT @GaugeDeviceAlarmGuid, @GaugeAlarm8130TemplateTagGuid, 'Device Alarm', 1, @AlarmApplicationStringGuid, 6, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @GaugeDeviceAlarmTemplateTagGuid, 1
	UNION ALL
	SELECT @GaugeConfigurationChangeAlarmGuid, @GaugeAlarm8130TemplateTagGuid, 'Configuration Change Alarm', 1, @AlarmApplicationStringGuid, 5, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @GaugeConfigurationChangeAlarmTemplateTagGuid, 1
	UNION ALL
	SELECT @GaugeRaiseFailureAlarmGuid, @GaugeAlarm8130TemplateTagGuid, 'Raise Failure Alarm', 1, @AlarmApplicationStringGuid, 4, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @GaugeRaiseFailureAlarmTemplateTagGuid, 1
	UNION ALL
	SELECT @GaugeLowerFailureAlarmGuid, @GaugeAlarm8130TemplateTagGuid, 'Lower Failure Alarm', 1, @AlarmApplicationStringGuid, 3, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @GaugeLowerFailureAlarmTemplateTagGuid, 1
	UNION ALL
	SELECT @GaugeUploadFailureAlarmGuid, @GaugeAlarm8130TemplateTagGuid, 'Upload Failure Alarm', 1, @AlarmApplicationStringGuid, 2, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @GaugeUploadFailureAlarmTemplateTagGuid, 1
	UNION ALL
	SELECT @GaugeDownloadFailureAlarmGuid, @GaugeAlarm8130TemplateTagGuid, 'Download Failure Alarm', 1, @AlarmApplicationStringGuid, 1, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @GaugeDownloadFailureAlarmTemplateTagGuid, 1
	UNION ALL
	SELECT @GaugeOverfillAlarmGuid, @GaugeAlarm8130TemplateTagGuid, 'Overfill Alarm', 1, @AlarmApplicationStringGuid, 0, 'Normal','Alarm Comment', null, null, 0, 0, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', @GaugeOverfillAlarmTemplateTagGuid, 1

	-- Leak Detection Alarms
	UNION ALL
	SELECT @LeakDetectionHighAlarmTemplateGuid,@LeakRateTagGuid,'Leak Detection High Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2023-04-05','Administrator','2023-04-05','Administrator',@LeakRateHighAlarmTagGuid,1
	UNION ALL
	SELECT @LeakDetectionDataInsufficientAlarmTemplateGuid,@LeakDetectionDiscreteAlarmTagGuid,'Leak Detection Data Insufficient Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2023-04-05','Administrator','2023-04-05','Administrator',@LeakDetectionAlarmTagGuid,1

	UNION ALL
	SELECT @VolumeGrossObservedHiHiAlarmTemplateGuid,@GrossObservedVolumeTemplateTagGuid,'Volume Gross Observed HiHi Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2023-11-21','Administrator','2023-11-21','Administrator',@VolumeGrossObservedHiHiAlarmTemplateTagGuid,1
	UNION ALL
	SELECT @VolumeGrossObservedHighAlarmTemplateGuid,@GrossObservedVolumeTemplateTagGuid,'Volume Gross Observed High Alarm',1,@AlarmApplicationStringGuid,1,'Normal','Alarm Comment',null,null,0,0,'2023-11-21','Administrator','2023-11-21','Administrator',@VolumeGrossObservedHighAlarmTemplateTagGuid,1
	UNION ALL
	SELECT @VolumeGrossObservedLowAlarmTemplateGuid,@GrossObservedVolumeTemplateTagGuid,'Volume Gross Observed Low Alarm',1,@AlarmApplicationStringGuid,1,'Normal','Alarm Comment',null,null,0,0,'2023-11-21','Administrator','2023-11-21','Administrator',@VolumeGrossObservedLowAlarmTemplateTagGuid,1
	UNION ALL
	SELECT @VolumeGrossObservedLoLoAlarmTemplateGuid,@GrossObservedVolumeTemplateTagGuid,'Volume Gross Observed LoLo Alarm',1,@AlarmApplicationStringGuid,0,'Normal','Alarm Comment',null,null,0,0,'2023-11-21','Administrator','2023-11-21','Administrator',@VolumeGrossObservedLoLoAlarmTemplateTagGuid,1

) 

AS Source
ON (Target.[AlarmTemplateGuid] = Source.[AlarmTemplateGuid])
WHEN MATCHED THEN
UPDATE SET target.[InputTemplateTagGuid] = source.[InputTemplateTagGuid]
		,target.[ID] = source.[ID]
		,target.[AlarmCategoryApplicationStringGuid] = source.[AlarmCategoryApplicationStringGuid]
		,target.[Order] = source.[Order]
		,target.[NotAlarmState] = source.[NotAlarmState]
		,target.[UpdatedDate] = SYSDATETIMEOFFSET()
		,target.[UpdatedBy] = source.[UpdatedBy]
		,target.[AlarmStateTemplateTagGuid] = source.[AlarmStateTemplateTagGuid]
		,target.[ExclusiveAlarm] = source.[ExclusiveAlarm]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([AlarmTemplateGuid]
			,[InputTemplateTagGuid]
			,[ID]
			,[Enabled]
			,[AlarmCategoryApplicationStringGuid]
			,[Order]
			,[NotAlarmState]
			,[Comment]
			,[ShelvedStartTimeStamp]
			,[ShelvedEndTimeStamp]
			,[ShelvedOneShot]
			,[Suppressed]
			,[CreatedDate]
			,[CreatedBy]
			,[UpdatedDate]
			,[UpdatedBy]
			,[AlarmStateTemplateTagGuid]
			,[ExclusiveAlarm])
	VALUES
			(source.[AlarmTemplateGuid]
			,source.[InputTemplateTagGuid]
			,source.[ID]
			,source.[Enabled]
			,source.[AlarmCategoryApplicationStringGuid]
			,source.[Order]
			,source.[NotAlarmState]
			,source.[Comment]
			,source.[ShelvedStartTimeStamp]
			,source.[ShelvedEndTimeStamp]
			,source.[ShelvedOneShot]
			,source.[Suppressed]
			,source.[CreatedDate]
			,source.[CreatedBy]
			,source.[UpdatedDate]
			,source.[UpdatedBy]
			,source.[AlarmStateTemplateTagGuid]
			,source.[ExclusiveAlarm]);

-- AlarmTestTemplate

DECLARE @NewLevelProductLowAlarmTestGuid UNIQUEIDENTIFIER = '2d3f942c-3857-4925-b79f-322f090cada7'
DECLARE @NewLevelProductLoLoAlarmTestGuid UNIQUEIDENTIFIER = 'b8e90490-b107-4e48-bf11-6db2a813286c'
DECLARE @NewLevelProductMinOpAlarmTestGuid UNIQUEIDENTIFIER = 'c31fe32e-e0fa-4c34-bfad-71c901458537'
DECLARE @NewLevelProductHighAlarmTestGuid UNIQUEIDENTIFIER = '778f6e56-82df-4868-b27e-09fe70d57a77'
DECLARE @NewLevelProductHiHiAlarmTestGuid UNIQUEIDENTIFIER = 'fb653ba6-6f3a-47c0-bd95-b7eb17124f37'
DECLARE @NewLevelProductMaxOpAlarmTestGuid UNIQUEIDENTIFIER = '3651e59b-6a28-4beb-8671-364318c1149b'

DECLARE @LevelProductLowAlarmTestGuid UNIQUEIDENTIFIER = '238aae81-fa0f-4a45-8af9-9d17d6f5dda0'
DECLARE @LevelProductLoLoAlarmTestGuid UNIQUEIDENTIFIER = 'b5f75cff-7307-45df-a8ba-a2b3d2732101'
DECLARE @LevelProductMinOpAlarmTestGuid UNIQUEIDENTIFIER = '3bca7585-09f9-4483-804b-4fe2b5dc90ba'
DECLARE @LevelProductHighAlarmTestGuid UNIQUEIDENTIFIER = 'f81b736d-c60e-420e-babc-303783fc2a3c'
DECLARE @LevelProductHiHiAlarmTestGuid UNIQUEIDENTIFIER = 'a6d2680c-51f1-49ae-b9c6-c92e949280bc'
DECLARE @LevelProductMaxOpAlarmTestGuid UNIQUEIDENTIFIER = '2d76b7d3-89c6-4dd0-aaf5-c8a7cee281fc'

DECLARE @TemperatureProductLowAlarmTestGuid UNIQUEIDENTIFIER = 'D3BB0106-66AE-424B-BE74-691F9534B96A'
DECLARE @TemperatureProductLoLoAlarmTestGuid UNIQUEIDENTIFIER = 'BDCCD3F7-8C6E-45A7-BEC0-1BD00E045B12'
DECLARE @TemperatureProductHighAlarmTestGuid UNIQUEIDENTIFIER = '94C732F1-B627-47D7-A574-246F32F175E1'
DECLARE @TemperatureProductHiHiAlarmTestGuid UNIQUEIDENTIFIER = 'B0B31EED-29E6-4A24-AF0F-4568D8DBE7B3'

DECLARE @VolumeGrossObservedLowAlarmTestGuid UNIQUEIDENTIFIER = 'C488D4FC-A73D-4BAB-A0D6-FE2E1BF6B4AD'
DECLARE @VolumeGrossObservedLoLoAlarmTestGuid UNIQUEIDENTIFIER = 'D92BF971-1946-488A-B5F5-B8FADAFD8E8C'
DECLARE @VolumeGrossObservedHighAlarmTestGuid UNIQUEIDENTIFIER = '41BD3719-586B-49C9-968E-97A8FB3716F5'
DECLARE @VolumeGrossObservedHiHiAlarmTestGuid UNIQUEIDENTIFIER = '21C88CCD-94DB-487E-8D62-AE7AC44A0BD4'

DECLARE @DensityProductLowAlarmTestGuid UNIQUEIDENTIFIER = 'F2088268-1FD6-490B-958B-FD13E7247111'
DECLARE @DensityProductHighAlarmTestGuid UNIQUEIDENTIFIER = '74F1A0D0-300E-49CD-8A11-19A7159D62D2'

DECLARE @TankModeMovementAlarmTestGuid UNIQUEIDENTIFIER = '7583057F-817A-4A5E-8EBC-F71E81DCC914'
DECLARE @TankModeReverseFlowAlarmTestGuid UNIQUEIDENTIFIER = '60F73DFF-B447-4F61-930B-7AF1C6C4B66F'
DECLARE @TankModeNoFlowAlarmTestGuid UNIQUEIDENTIFIER = 'E0DE1861-FA7D-4D6C-B351-B12C5B3CEC79'
DECLARE @TankModeTestingAlarmTestGuid UNIQUEIDENTIFIER = '33A4BC67-EAAD-4121-8DF7-52B54D323E2E'

DECLARE @TankTransferAdvisoryAlarmTestGuid UNIQUEIDENTIFIER = '601D03E4-958B-47FD-80DE-816A10444DF6'
DECLARE @TankTransferShutdownAlarmTestGuid UNIQUEIDENTIFIER = 'B1E1149D-EEE3-44C0-86A8-87F0E35FC33C'

-- Level Product Rate Alarm Emptying/Filling Test
DECLARE @LevelProductRateAlarmEmptyingTestGuid UNIQUEIDENTIFIER = '49BB823B-5714-47F0-8C2C-0724EC081232'
DECLARE @LevelProductRateAlarmFillingTestGuid UNIQUEIDENTIFIER = '03905969-24C5-4323-AA00-9F81DC814A1B'

-- Volume Total Observed Rate Alarm Emptying/Filling Test
DECLARE @VolumeTotalObservedRateAlarmEmptyingTestGuid UNIQUEIDENTIFIER = '467C005B-8F3C-429A-889F-70303F118991'
DECLARE @VolumeTotalObservedRateAlarmFillingTestGuid UNIQUEIDENTIFIER = 'F4C52D5D-0B79-4F17-BE32-32B35A594E10'

-- Volume Net Standard Rate Alarm Emptying/Filling Test
DECLARE @VolumeNetStandardRateAlarmEmptyingTestGuid UNIQUEIDENTIFIER = '4D8A1A58-790D-4CAC-91CC-5B2CECAE30FF'
DECLARE @VolumeNetStandardRateAlarmFillingTestGuid UNIQUEIDENTIFIER = '6A3356E7-CE4D-4675-A6CB-083DEEC40C08'

-- Volume Gross Observed Rate Alarm Emptying/Filling Test
DECLARE @VolumeGrossObservedRateAlarmEmptyingTestGuid UNIQUEIDENTIFIER = 'E61CBFEA-0BDD-49AB-AC15-3B71D34C8537'
DECLARE @VolumeGrossObservedRateAlarmFillingTestGuid UNIQUEIDENTIFIER = '5DBF14EC-0E1B-49E1-8C01-1A250421221E'

-- Gauge Alarm Test
DECLARE @GaugeScanFailureAlarmTestGuid UNIQUEIDENTIFIER = '16EF61F4-2754-4D0D-A752-9089A1246207'
DECLARE @GaugeInputOutputFailureAlarmTestGuid UNIQUEIDENTIFIER = '745B6CB5-C7DE-4C47-BC90-428759A2066A'
DECLARE @GaugeDeviceAlarmTestGuid UNIQUEIDENTIFIER = '3BEFA2E7-E9E5-46BD-A155-E4B0307E55D0'
DECLARE @GaugeConfigurationChangeAlarmTestGuid UNIQUEIDENTIFIER = '0D9C9798-BF87-4AE8-8D3F-098F0BBB96D0'
DECLARE @GaugeRaiseFailureAlarmTestGuid UNIQUEIDENTIFIER = '0783A9E4-0508-49DA-A2FB-148C5172881E'
DECLARE @GaugeLowerFailureAlarmTestGuid UNIQUEIDENTIFIER = '43327167-484A-400D-BDB1-AC189FB3F9EE'
DECLARE @GaugeUploadFailureAlarmTestGuid UNIQUEIDENTIFIER = '0B11D9C3-3BDB-4C0D-9F97-E688DE414FDB'
DECLARE @GaugeDownloadFailureAlarmTestGuid UNIQUEIDENTIFIER = '87D90CBF-A01F-4CFF-9438-4862F9F34108'
DECLARE @GaugeOverfillAlarmTestGuid UNIQUEIDENTIFIER = '9BE2E3A3-5EA8-4AD4-BB35-64F8801FEFB2'

-- Leak Detection Alarm Test
DECLARE @LeakRateHighAlarmTestGuid UNIQUEIDENTIFIER = '0DACFF13-C243-4329-BA0C-34FB05A665CE'
DECLARE @LeakDetectionAlarmTestGuid UNIQUEIDENTIFIER = '4480ACC9-3DB9-4C57-B1B7-66AF80855EC1'

MERGE dbo.tblAlarmTestTemplate AS Target
USING 
(  SELECT @LevelProductHiHiAlarmTestGuid AS [AlarmTestTemplateGuid]
		,@LevelHiHiAlarmTemplateGuid AS [AlarmTemplateGuid]
		,'HiHi Test' AS [ID]
		,@LevelHiHiLimitTemplateTagGuid AS [LimitTemplateTagGuid]
		,0 AS [TagField]  -- enum TagFieldEnum { Value = 0, Status = 1, OpcStatusSubCode = 2 }
		,@HiHiLoLoAlarmPriorityGuid AS [AlarmPriorityGuid]
		,@NormalUnacknowledgedAlarmPriorityGuid as [NormalUnacknowledgedAlarmPriorityGuid]
		,1 AS [TestType]  -- enum TestTypeEnum { GreaterThan = 0, GreaterThanOrEqual = 1, LessThan = 2, LessThanOrEqual = 3, Equals = 4, NotEquals = 5 }
		,-1 AS [BitMask]
		,1 AS [Enabled]
		,0 AS [Order]
		,'HiHi Alarm' AS [AlarmState]
		,0.00 AS [Holdoff]
		,'Level Product HiHi Alarm' AS [AlarmText]
		,null AS [HelpFile]
		,null AS [DrawingGuid]
		,'2015-02-04' as [CreatedDate]
		,'Administrator' as [CreatedBy]
		,'2015-02-04' as [UpdatedDate]
		,'Administrator' as [UpdatedBy]
		,0 as [BitwiseOperator]  -- enum BitwiseOperatorEnum { And = 0, Or = 1, Nor = 3, Xor = 4, Nand = 5, Nxor = 6 }
		,0 as [TimedHoldOffInSeconds]
		UNION ALL
		SELECT @LevelProductHighAlarmTestGuid,@LevelHighAlarmTemplateGuid,'High Test',@LevelHighLimitTemplateTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,1,-1,1,0,'High Alarm',0.00,'Level Product High Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @LevelProductMaxOpAlarmTestGuid,@LevelMaxOpAlarmTemplateGuid,'MaxOp Test',@LevelMaxOpLimitTemplateTagGuid,0,@MaxMinOperatingAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,1,-1,1,0,'Max Op Alarm',0.00,'Level Product Max Op Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @LevelProductMinOpAlarmTestGuid,@LevelMinOpAlarmTemplateGuid,'MinOp Test',@LevelMinOpLimitTemplateTagGuid,0,@MaxMinOperatingAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,3,-1,1,0,'Min Op Alarm',0.00,'Level Product Min Op Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @LevelProductLowAlarmTestGuid,@LevelLowAlarmTemplateGuid,'Low Test',@LevelLowLimitTemplateTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,3,-1,1,0,'Low Alarm',0.00,'Level Product Low Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @LevelProductLoLoAlarmTestGuid,@LevelLoLoAlarmTemplateGuid,'LoLo Test',@LevelLoLoLimitTemplateTagGuid,0,@HiHiLoLoAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,3,-1,1,0,'LoLo Alarm',0.00,'Level Product LoLo Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @TemperatureProductHiHiAlarmTestGuid,@TemperatureHiHiAlarmTemplateGuid,'HiHi Test',@TemperatureHiHiLimitTemplateTagGuid,0,@HiHiLoLoAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,1,-1,1,0,'HiHi Alarm',0.00,'Temperature Product HiHi Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @TemperatureProductHighAlarmTestGuid,@TemperatureHighAlarmTemplateGuid,'High Test',@TemperatureHighLimitTemplateTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,1,-1,1,0,'High Alarm',0.00,'Temperature Product High Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @TemperatureProductLowAlarmTestGuid,@TemperatureLowAlarmTemplateGuid,'Low Test',@TemperatureLowLimitTemplateTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,3,-1,1,0,'Low Alarm',0.00,'Temperature Product Low Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @TemperatureProductLoLoAlarmTestGuid,@TemperatureLoLoAlarmTemplateGuid,'LoLo Test',@TemperatureLoLoLimitTemplateTagGuid,0,@HiHiLoLoAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,3,-1,1,0,'LoLo Alarm',0.00,'Temperature Product LoLo Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @DensityProductHighAlarmTestGuid,@DensityHighAlarmTemplateGuid,'High Test',@DensityHighLimitTemplateTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,1,-1,1,0,'High Alarm',0.00,'Density Product Observed High Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @DensityProductLowAlarmTestGuid,@DensityLowAlarmTemplateGuid,'Low Test',@DensityLowLimitTemplateTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,3,-1,1,0,'Low Alarm',0.00,'Density Product Observed Low Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @TankModeMovementAlarmTestGuid,@TankModeDiscreteAlarmGuid,'Movement Test',@TankModeDiscreteAlarmMovementTestTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x01', 1)),1,0,'Movement',0.00,'Tank Mode Movement Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @TankModeReverseFlowAlarmTestGuid,@TankModeDiscreteAlarmGuid,'Reverse Flow Test',@TankModeDiscreteAlarmReverseFlowTestTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x02', 1)),1,1,'Reverse Flow',0.00,'Tank Mode Reverse Flow Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @TankModeNoFlowAlarmTestGuid,@TankModeDiscreteAlarmGuid,'No Flow Test',@TankModeDiscreteAlarmNoFlowTestTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x04', 1)),1,2,'No Flow',0.00,'Tank Mode No Flow Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @TankModeTestingAlarmTestGuid,@TankModeDiscreteAlarmGuid,'Testing Test',@TankModeDiscreteAlarmTesttingTestTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x08', 1)),1,3,'Testing',0.00,'Tank Mode Testing Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @TankTransferAdvisoryAlarmTestGuid,@TankTransferAdvisoryDiscreteAlarmGuid,'Transfer Advisory Test',@TransferAdvisoryAlarmLimitTemplateTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x10', 1)),1,0,'Transfer Advisory',0.00,'Transfer Advisory Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @TankTransferShutdownAlarmTestGuid,@TankTransferTargetDiscreteAlarmGuid,'Transfer Target Test',@TransferShutdownAlarmLimitTemplateTagGuid,0,@HiHiLoLoAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x20', 1)),1,1,'Transfer Target',0.00,'Transfer Target Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0

		-- Level Product Rate Alarm Emptying Test
		UNION ALL
		SELECT @LevelProductRateAlarmEmptyingTestGuid, @LevelProductRateAlarmEmptyingTemplateGuid, 'Level Product Rate Emptying Test', @LevelProductRateAlarmLimitEmptyingTemplateTagGuid, 0, @HighLowAlarmPriorityGuid, @NormalUnacknowledgedAlarmPriorityGuid, 2, -1, 1, 0, 'Emptying Alarm', 0.00, 'Level Product Rate Emptying Alarm', null, null, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', 0, 0
		UNION ALL
		SELECT @LevelProductRateAlarmFillingTestGuid, @LevelProductRateAlarmFillingTemplateGuid, 'Level Product Rate Filling Test', @LevelProductRateAlarmLimitFillingTemplateTagGuid, 0, @HighLowAlarmPriorityGuid, @NormalUnacknowledgedAlarmPriorityGuid, 0, -1, 1, 0, 'Filling Alarm', 0.00, 'Level Product Rate Filling Alarm', null, null, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', 0, 0

		-- Volume Total Observed Rate Alarm Emptying Test
		UNION ALL
		SELECT @VolumeTotalObservedRateAlarmEmptyingTestGuid, @VolumeTotalObservedRateAlarmEmptyingTemplateGuid, 'Volume Total Observed Rate Emptying Test', @VolumeTotalObservedRateAlarmLimitEmptyingTemplateTagGuid, 0, @HighLowAlarmPriorityGuid, @NormalUnacknowledgedAlarmPriorityGuid, 3, -1, 1, 0, 'Emptying Alarm', 0.00, 'Volume Total Observed Rate Emptying Alarm', null, null, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', 0, 0
		UNION ALL
		SELECT @VolumeTotalObservedRateAlarmFillingTestGuid, @VolumeTotalObservedRateAlarmFillingTemplateGuid, 'Volume Total Observed Rate Filling Test', @VolumeTotalObservedRateAlarmLimitFillingTemplateTagGuid, 0, @HighLowAlarmPriorityGuid, @NormalUnacknowledgedAlarmPriorityGuid, 1, -1, 1, 0, 'Filling Alarm', 0.00, 'Volume Total Observed Rate Filling Alarm', null, null, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', 0, 0
		
		-- Volume Net Standard Rate Alarm Emptying Test
		UNION ALL
		SELECT @VolumeNetStandardRateAlarmEmptyingTestGuid, @VolumeNetStandardRateAlarmEmptyingTemplateGuid, 'Volume Net Standard Rate Emptying Test', @VolumeNetStandardRateAlarmLimitEmptyingTemplateTagGuid, 0, @HighLowAlarmPriorityGuid, @NormalUnacknowledgedAlarmPriorityGuid, 3, -1, 1, 0, 'Emptying Alarm', 0.00, 'Volume Net Standard Rate Emptying Alarm', null, null, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', 0, 0
		UNION ALL
		SELECT @VolumeNetStandardRateAlarmFillingTestGuid, @VolumeNetStandardRateAlarmFillingTemplateGuid, 'Volume Net Standard Rate Filling Test', @VolumeNetStandardRateAlarmLimitFillingTemplateTagGuid, 0, @HighLowAlarmPriorityGuid, @NormalUnacknowledgedAlarmPriorityGuid, 1, -1, 1, 0, 'Filling Alarm', 0.00, 'Volume Net Standard Rate Filling Alarm', null, null, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', 0, 0

		-- Volume Gross Observed Rate Alarm Emptying Test
		UNION ALL
		SELECT @VolumeGrossObservedRateAlarmEmptyingTestGuid, @VolumeGrossObservedRateAlarmEmptyingTemplateGuid, 'Volume Gross Observed Rate Emptying Test', @VolumeGrossObservedRateAlarmLimitEmptyingTemplateTagGuid, 0, @HighLowAlarmPriorityGuid, @NormalUnacknowledgedAlarmPriorityGuid, 3, -1, 1, 0, 'Emptying Alarm', 0.00, 'Volume Gross Observed Rate Emptying Alarm', null, null, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', 0, 0
		UNION ALL
		SELECT @VolumeGrossObservedRateAlarmFillingTestGuid, @VolumeGrossObservedRateAlarmFillingTemplateGuid, 'Volume Gross Observed Rate Filling Test', @VolumeGrossObservedRateAlarmLimitFillingTemplateTagGuid, 0, @HighLowAlarmPriorityGuid, @NormalUnacknowledgedAlarmPriorityGuid, 1, -1, 1, 0, 'Filling Alarm', 0.00, 'Volume Gross Observed Rate Filling Alarm', null, null, '2017-09-07', 'Administrator', '2017-09-07', 'Administrator', 0, 0

		-- Gauge Alarm Test
		UNION ALL
		SELECT @GaugeScanFailureAlarmTestGuid,@GaugeScanFailureAlarmGuid,'Scan Failure Test',@GaugeScanFailureLimitTemplateTagGuid,0,@MaxMinOperatingAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x01', 1)),1,1,'Scan Failure Alarm',0.00,'Gauge Alarm Scan Failure Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @GaugeInputOutputFailureAlarmTestGuid,@GaugeInputOutputFailureAlarmGuid,'Input Output Failure Test',@GaugeInputOutputFailureLimitTemplateTagGuid,0,@MaxMinOperatingAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x02', 1)),1,1,'Input Output Failure Alarm',0.00,'Gauge Alarm I/O Failure Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @GaugeDeviceAlarmTestGuid,@GaugeDeviceAlarmGuid,'Device Alarm Test',@GaugeDeviceLimitTemplateTagGuid,0,@MaxMinOperatingAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x04', 1)),1,1,'Device Alarm',0.00,'Gauge Alarm Device Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @GaugeConfigurationChangeAlarmTestGuid,@GaugeConfigurationChangeAlarmGuid,'Configuration Change Test',@GaugeConfigurationChangeLimitTemplateTagGuid,0,@MaxMinOperatingAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x08', 1)),1,1,'Configuration Change Alarm',0.00,'Gauge Alarm Configuration Change Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @GaugeRaiseFailureAlarmTestGuid,@GaugeRaiseFailureAlarmGuid,'Raise Failure Test',@GaugeRaiseFailureLimitTemplateTagGuid,0,@MaxMinOperatingAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x10', 1)),1,1,'Raise Failure Alarm',0.00,'Gauge Alarm Raise Failure Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @GaugeLowerFailureAlarmTestGuid,@GaugeLowerFailureAlarmGuid,'Lower Failure Test',@GaugeLowerFailureLimitTemplateTagGuid,0,@MaxMinOperatingAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x20', 1)),1,1,'Lower Failure Alarm',0.00,'Gauge Alarm Lower Failure Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @GaugeUploadFailureAlarmTestGuid,@GaugeUploadFailureAlarmGuid,'Upload Failure Test',@GaugeUploadFailureLimitTemplateTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x40', 1)),1,1,'Upload Failure Alarm',0.00,'Gauge Alarm Upload Failure Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION 
		SELECT @GaugeDownloadFailureAlarmTestGuid,@GaugeDownloadFailureAlarmGuid,'Download Failure Test',@GaugeDownloadFailureLimitTemplateTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x80', 1)),1,1,'Download Failure Alarm',0.00,'Gauge Alarm Download Failure Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0
		UNION ALL
		SELECT @GaugeOverfillAlarmTestGuid,@GaugeOverfillAlarmGuid,'Overfill Test',@GaugeOverfillLimitTemplateTagGuid,0,@HiHiLoLoAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x0100', 1)),1,1,'Overfill Alarm',0.00,'Gauge Alarm Overfill Alarm',null,null,'2015-02-04','Administrator','2015-02-04','Administrator',0,0

		-- Leak Detection Alarm Test
		UNION ALL
		SELECT @LeakRateHighAlarmTestGuid,@LeakDetectionHighAlarmTemplateGuid,'High Test',@LeakRateHighLimitTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,1,-1,1,0,'High Alarm',0.00,'Leak Detection Rate High Alarm',null,null,'2023-04-05','Administrator','2023-04-05','Administrator',0,0
		UNION ALL
		SELECT @LeakDetectionAlarmTestGuid,@LeakDetectionDataInsufficientAlarmTemplateGuid,'Data Insufficient Test',@LeakDetectionDataInsufficientLimitTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,4,CONVERT(INT, CONVERT(VARBINARY, '0x01', 1)),1,0,'Low Alarm',0.00,'Leak Detection Data Insufficient Limit Alarm',null,null,'2023-04-05','Administrator','2023-04-05','Administrator',0,0

		-- Volume Gross Observed Alarm Test
		UNION ALL
		SELECT @VolumeGrossObservedHiHiAlarmTestGuid,@VolumeGrossObservedHiHiAlarmTemplateGuid,'HiHi Test',@VolumeGrossObservedHiHiLimitTemplateTagGuid,0,@HiHiLoLoAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,1,-1,1,0,'HiHi Alarm',0.00,'Volume Gross Observed HiHi Alarm',null,null,'2023-11-21','Administrator','2023-11-21','Administrator',0,0
		UNION ALL
		SELECT @VolumeGrossObservedHighAlarmTestGuid,@VolumeGrossObservedHighAlarmTemplateGuid,'High Test',@VolumeGrossObservedHighLimitTemplateTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,1,-1,1,0,'High Alarm',0.00,'Volume Gross Observed High Alarm',null,null,'2023-11-21','Administrator','2023-11-21','Administrator',0,0
		UNION ALL
		SELECT @VolumeGrossObservedLowAlarmTestGuid,@VolumeGrossObservedLowAlarmTemplateGuid,'Low Test',@VolumeGrossObservedLowLimitTemplateTagGuid,0,@HighLowAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,3,-1,1,0,'Low Alarm',0.00,'Volume Gross Observed Low Alarm',null,null,'2023-11-21','Administrator','2023-11-21','Administrator',0,0
		UNION ALL
		SELECT @VolumeGrossObservedLoLoAlarmTestGuid,@VolumeGrossObservedLoLoAlarmTemplateGuid,'LoLo Test',@VolumeGrossObservedLoLoLimitTemplateTagGuid,0,@HiHiLoLoAlarmPriorityGuid,@NormalUnacknowledgedAlarmPriorityGuid,3,-1,1,0,'LoLo Alarm',0.00,'Volume Gross Observed LoLo Alarm',null,null,'2023-11-21','Administrator','2023-11-21','Administrator',0,0
		
) 
AS Source
ON (Target.[AlarmTestTemplateGuid] = Source.[AlarmTestTemplateGuid])
WHEN MATCHED THEN
UPDATE SET target.[AlarmTemplateGuid] = source.[AlarmTemplateGuid]
		,target.[ID] = source.[ID]
		,target.[LimitTemplateTagGuid] = source.[LimitTemplateTagGuid]
		,target.[TagField] = source.[TagField]  -- enum TagFieldEnum { Value = 0, Status = 1, OpcStatusSubCode = 2 }
		,target.[AlarmPriorityGuid] = source.[AlarmPriorityGuid]
		,target.[NormalUnacknowledgedAlarmPriorityGuid] = source.[NormalUnacknowledgedAlarmPriorityGuid]
		,target.[TestType] = source.[TestType]  -- enum TestTypeEnum { GreaterThan = 0, GreaterThanOrEqual = 1, LessThan = 2, LessThanOrEqual = 3, Equals = 4, NotEquals = 5 }
		,target.[BitMask] = source.[BitMask]
		,target.[Order] = source.[Order]
		,target.[AlarmState] = source.[AlarmState]
		,target.[AlarmText] = source.[AlarmText]
		,target.[UpdatedDate] = SYSDATETIMEOFFSET()
		,target.[UpdatedBy] = source.[UpdatedBy]
		,target.[BitwiseOperator] = source.[BitwiseOperator]  -- enum BitwiseOperatorEnum { And = 0, Or = 1, Nor = 3, Xor = 4, Nand = 5, Nxor = 6 }
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([AlarmTestTemplateGuid]
			,[AlarmTemplateGuid]
			,[ID]
			,[LimitTemplateTagGuid]
			,[TagField]
			,[AlarmPriorityGuid]
			,[NormalUnacknowledgedAlarmPriorityGuid]
			,[TestType]
			,[BitMask]
			,[Enabled]
			,[Order]
			,[AlarmState]
			,[Holdoff]
			,[AlarmText]
			,[HelpFile]
			,[DrawingGuid]
			,[CreatedDate]
			,[CreatedBy]
			,[UpdatedDate]
			,[UpdatedBy]
			,[BitwiseOperator]
			,[TimedHoldOffInSeconds])
	VALUES
			(source.[AlarmTestTemplateGuid]
			,source.[AlarmTemplateGuid]
			,source.[ID]
			,source.[LimitTemplateTagGuid]
			,source.[TagField]
			,source.[AlarmPriorityGuid]
			,source.[NormalUnacknowledgedAlarmPriorityGuid]
			,source.[TestType]
			,source.[BitMask]
			,source.[Enabled]
			,source.[Order]
			,source.[AlarmState]
			,source.[Holdoff]
			,source.[AlarmText]
			,source.[HelpFile]
			,source.[DrawingGuid]
			,source.[CreatedDate]
			,source.[CreatedBy]
			,source.[UpdatedDate]
			,source.[UpdatedBy]
			,source.[BitwiseOperator]
			,source.[TimedHoldOffInSeconds]);

--PointTemplateTagStatus

DECLARE @LevelProductLowAlarmStatusGuid UNIQUEIDENTIFIER = 'cd177261-ebb8-4012-bd19-c571b51af9df'
DECLARE @LevelProductLoLoAlarmStatusGuid UNIQUEIDENTIFIER = 'b4ee9a46-2f22-495e-8a75-dfa8be11a9b6'
DECLARE @LevelProductMinOpAlarmStatusGuid UNIQUEIDENTIFIER = '9c4e0cfd-8c98-4977-b74d-4cf6d06085ed'
DECLARE @LevelProductHighAlarmStatusGuid UNIQUEIDENTIFIER = '509eb823-0ed8-47e3-8e25-484041c6fd2b'
DECLARE @LevelProductHiHiAlarmStatusGuid UNIQUEIDENTIFIER = '6adf0135-69ad-480c-8b05-3a206bd50ff5'
DECLARE @LevelProductMaxOpAlarmStatusGuid UNIQUEIDENTIFIER = '612cc3ec-af36-46ce-a01d-6a76b63980a7'

DECLARE @TemperatureProductLowAlarmStatusGuid UNIQUEIDENTIFIER = '2EB1C2C7-49CF-4BF6-A672-C4A4A3B5005F'
DECLARE @TemperatureProductLoLoAlarmStatusGuid UNIQUEIDENTIFIER = '8A9DE161-6B26-4989-953B-3686B8B8F04E'
DECLARE @TemperatureProductHighAlarmStatusGuid UNIQUEIDENTIFIER = '9D2906F7-A852-472A-A823-2C5C32B648A1'
DECLARE @TemperatureProductHiHiAlarmStatusGuid UNIQUEIDENTIFIER = 'EC7286AC-7A45-404D-9340-27AF7BF786E2'

DECLARE @VolumeGrossObservedLowAlarmStatusGuid UNIQUEIDENTIFIER = 'A53FAB39-FF30-4D19-96C3-4172F2C9CF3D'
DECLARE @VolumeGrossObservedLoLoAlarmStatusGuid UNIQUEIDENTIFIER = '8335A55D-12D3-45C0-A400-44FD4E5D72C7'
DECLARE @VolumeGrossObservedHighAlarmStatusGuid UNIQUEIDENTIFIER = '2EF8F99C-A31F-4CB9-B3B1-7917D00BC39C'
DECLARE @VolumeGrossObservedHiHiAlarmStatusGuid UNIQUEIDENTIFIER = '93427427-A226-45AC-9139-B36503907371'

DECLARE @DensityProductLowAlarmStatusGuid UNIQUEIDENTIFIER = '9FD4A440-C31E-431D-A4D3-19DA185A2E3C'
DECLARE @DensityProductHighAlarmStatusGuid UNIQUEIDENTIFIER = 'D8A66235-2881-40B9-908E-8F62E22FF274'

DECLARE @TankModeMovementAlarmStatusGuid UNIQUEIDENTIFIER = '6FC117F6-849F-4A11-A478-C4C497808E92'
DECLARE @TankModeReverseFlowAlarmStatusGuid UNIQUEIDENTIFIER = '009DCA1A-55CC-4C78-B69D-123A6162327F'
DECLARE @TankModeNoFlowAlarmStatusGuid UNIQUEIDENTIFIER = '416ADD07-D635-4529-9D99-50D84EB576EC'
DECLARE @TankModeTestingAlarmStatusGuid UNIQUEIDENTIFIER = '7FE0EEC4-E42E-46BB-B237-7D4C676DFC74'

DECLARE @TankTransferAdvisoryAlarmStatusGuid UNIQUEIDENTIFIER = '1A8F7DF5-6D2E-4F37-8741-FF7E54F05644'
DECLARE @TankTransferShutdownAlarmStatusGuid UNIQUEIDENTIFIER = '5F549E5C-D040-4586-8D53-D0ECE8221D33'

-- Level Product Rate Alarm emptying/filling status
DECLARE @LevelProductRateAlarmEmptyingStatusGuid UNIQUEIDENTIFIER = 'FAA04D5D-0D4B-4C52-9967-525D3B09CA96'
DECLARE @LevelProductRateAlarmFillingStatusGuid UNIQUEIDENTIFIER = 'B2828C1C-6C4B-4FCD-B182-54FD7B6451FB'

-- Volume Total Observed Rate Alarm emptying/filling status
DECLARE @VolumeTotalObservedRateAlarmEmptyingStatusGuid UNIQUEIDENTIFIER = 'F7629065-F5A6-4674-8DB7-5064AE987371'
DECLARE @VolumeTotalObservedRateAlarmFillingStatusGuid UNIQUEIDENTIFIER = '83269204-64A0-4241-87BB-C1ACB2A23EA6'

-- Volume Net Standard Rate Alarm emptying/filling status
DECLARE @VolumeNetStandardRateAlarmEmptyingStatusGuid UNIQUEIDENTIFIER = '8C3C81B0-FFE6-41A3-A9F4-09344E8EA04F'
DECLARE @VolumeNetStandardRateAlarmFillingStatusGuid UNIQUEIDENTIFIER = 'A578B106-CE7A-4562-87E8-113354862071'

-- Volume Gross Observed Rate Alarm emptying/filling status
DECLARE @VolumeGrossObservedRateAlarmEmptyingStatusGuid UNIQUEIDENTIFIER = '448C7C73-1ECC-4C24-900C-689D6DA818FF'
DECLARE @VolumeGrossObservedRateAlarmFillingStatusGuid UNIQUEIDENTIFIER = 'C4B9302E-94CE-4891-B157-1F55E3409CF1'

-- Gauge Alarm Status
DECLARE @GaugeScanFailureAlarmStatusGuid UNIQUEIDENTIFIER = 'BC6A87BB-B71B-4E7D-89E9-2100AA059459'
DECLARE @GaugeInputOutputFailureAlarmStatusGuid UNIQUEIDENTIFIER = '8B576208-5DAF-4A80-BA6B-0FF88D6CBDBC'
DECLARE @GaugeDeviceAlarmStatusGuid UNIQUEIDENTIFIER = 'AC081F77-CAB2-4E51-A25B-0CFF785476FE'
DECLARE @GaugeConfigurationChangeAlarmStatusGuid UNIQUEIDENTIFIER = '7A519F00-9DAC-458C-9F1C-E5C189EFD0DC'
DECLARE @GaugeRaiseFailureAlarmStatusGuid UNIQUEIDENTIFIER = 'F03BF2F9-2A67-4601-A4A1-365F20B401F9'
DECLARE @GaugeLowerFailureAlarmStatusGuid UNIQUEIDENTIFIER = '38E5E687-52F7-4138-839B-0518AE7FE8B2'
DECLARE @GaugeUploadFailureAlarmStatusGuid UNIQUEIDENTIFIER = '9FA848B2-D88B-4FC1-92CB-9BAD3DA5B5D8'
DECLARE @GaugeDownloadFailureAlarmStatusGuid UNIQUEIDENTIFIER = '91D02A79-B52B-480B-ABDA-C3A27827524F'
DECLARE @GaugeOverfillAlarmStatusGuid UNIQUEIDENTIFIER = '8485E32F-3241-44BA-8882-34CF8B2315EC'

-- Leak Detection Alarm Status
DECLARE @LeakRateHighAlarmStatusGuid UNIQUEIDENTIFIER = '2FF3886F-8E60-4553-ADFF-23013658B7D1'
DECLARE @LeakDetectionAlarmStatusGuid UNIQUEIDENTIFIER = 'B3829E7E-4840-422F-9608-C6686345B800'

MERGE dbo.tblPointTemplateTagAlarmStatus AS Target
USING 
(  SELECT @LevelProductHiHiAlarmStatusGuid AS [PointTemplateTagAlarmStatusGuid],
	@LevelProductHiHiAlarmTestGuid AS[AlarmTestTemplateGuid],
	1 AS [Acknowledged],
	null AS [AcknowledgedTimestamp],
	null AS [AcknowledgedBy],
	null AS [AcknowledgedComment],
	1 AS [Silenced],
	null as [SilencedTimestamp],
	null as [SilencedBy],
	0 AS [AlarmTestFailed],
	null AS [AlarmTestFailedTimestamp],
	'2015-02-04' as [CreatedDate],
	'Administrator' as [CreatedBy],
	'2015-02-04' as [UpdatedDate],
	'Administrator' as [UpdatedBy]
		UNION ALL
		SELECT @LevelProductHighAlarmStatusGuid,@LevelProductHighAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @LevelProductMaxOpAlarmStatusGuid,@LevelProductMaxOpAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @LevelProductMinOpAlarmStatusGuid,@LevelProductMinOpAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @LevelProductLowAlarmStatusGuid,@LevelProductLowAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @LevelProductLoLoAlarmStatusGuid,@LevelProductLoLoAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @TemperatureProductHiHiAlarmStatusGuid,@TemperatureProductHiHiAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @TemperatureProductHighAlarmStatusGuid,@TemperatureProductHighAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @TemperatureProductLowAlarmStatusGuid,@TemperatureProductLowAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @TemperatureProductLoLoAlarmStatusGuid,@TemperatureProductLoLoAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @DensityProductHighAlarmStatusGuid,@DensityProductHighAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @DensityProductLowAlarmStatusGuid,@DensityProductLowAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @TankModeMovementAlarmStatusGuid,@TankModeMovementAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @TankModeReverseFlowAlarmStatusGuid,@TankModeReverseFlowAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @TankModeNoFlowAlarmStatusGuid,@TankModeNoFlowAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @TankModeTestingAlarmStatusGuid,@TankModeTestingAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @TankTransferAdvisoryAlarmStatusGuid,@TankTransferAdvisoryAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'
		UNION ALL
		SELECT @TankTransferShutdownAlarmStatusGuid,@TankTransferShutdownAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2015-02-04','Administrator','2015-02-04','Administrator'

		-- Level Product Rate Alarm status
		UNION ALL
		SELECT @LevelProductRateAlarmEmptyingStatusGuid, @LevelProductRateAlarmEmptyingTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'
		UNION ALL
		SELECT @LevelProductRateAlarmFillingStatusGuid, @LevelProductRateAlarmFillingTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'

		-- Volume Total Observed Rate Alarm status
		UNION ALL
		SELECT @VolumeTotalObservedRateAlarmEmptyingStatusGuid, @VolumeTotalObservedRateAlarmEmptyingTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'
		UNION ALL
		SELECT @VolumeTotalObservedRateAlarmFillingStatusGuid, @VolumeTotalObservedRateAlarmFillingTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'

		-- Volume Net Standard Rate Alarm status
		UNION ALL
		SELECT @VolumeNetStandardRateAlarmEmptyingStatusGuid, @VolumeNetStandardRateAlarmEmptyingTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'
		UNION ALL
		SELECT @VolumeNetStandardRateAlarmFillingStatusGuid, @VolumeNetStandardRateAlarmFillingTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'

		-- Volume Gross Observed Rate Alarm status
		UNION ALL
		SELECT @VolumeGrossObservedRateAlarmEmptyingStatusGuid, @VolumeGrossObservedRateAlarmEmptyingTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'
		UNION ALL
		SELECT @VolumeGrossObservedRateAlarmFillingStatusGuid, @VolumeGrossObservedRateAlarmFillingTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'


		-- Gauge Alarm Status
		UNION ALL
		SELECT @GaugeScanFailureAlarmStatusGuid, @GaugeScanFailureAlarmTestGuid, 1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'
		UNION ALL
		SELECT @GaugeInputOutputFailureAlarmStatusGuid, @GaugeInputOutputFailureAlarmTestGuid, 1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'
		UNION ALL
		SELECT @GaugeDeviceAlarmStatusGuid, @GaugeDeviceAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'
		UNION ALL
		SELECT @GaugeConfigurationChangeAlarmStatusGuid, @GaugeConfigurationChangeAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'
		UNION ALL
		SELECT @GaugeRaiseFailureAlarmStatusGuid, @GaugeRaiseFailureAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'
		UNION ALL
		SELECT @GaugeLowerFailureAlarmStatusGuid, @GaugeLowerFailureAlarmTestGuid, 1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'
		UNION ALL
		SELECT @GaugeUploadFailureAlarmStatusGuid, @GaugeUploadFailureAlarmTestGuid, 1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'
		UNION ALL
		SELECT @GaugeDownloadFailureAlarmStatusGuid, @GaugeDownloadFailureAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'
		UNION ALL
		SELECT @GaugeOverfillAlarmStatusGuid, @GaugeOverfillAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2017-09-07','Administrator','2017-09-07','Administrator'

		-- Leak Detection Alarm Status
		UNION ALL
		SELECT @LeakRateHighAlarmStatusGuid,@LeakRateHighAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2023-04-05','Administrator','2023-04-05','Administrator'
		UNION ALL
		SELECT @LeakDetectionAlarmStatusGuid,@LeakDetectionAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2023-04-05','Administrator','2023-04-05','Administrator'

		-- Volume Gross Observed Status
		UNION ALL
		SELECT @VolumeGrossObservedHiHiAlarmStatusGuid,@VolumeGrossObservedHiHiAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2023-11-21','Administrator','2023-11-21','Administrator'
		UNION ALL
		SELECT @VolumeGrossObservedHighAlarmStatusGuid,@VolumeGrossObservedHighAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2023-11-21','Administrator','2023-11-21','Administrator'
		UNION ALL
		SELECT @VolumeGrossObservedLowAlarmStatusGuid,@VolumeGrossObservedLowAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2023-11-21','Administrator','2023-11-21','Administrator'
		UNION ALL
		SELECT @VolumeGrossObservedLoLoAlarmStatusGuid,@VolumeGrossObservedLoLoAlarmTestGuid,1,null,null,null,1,null,null,0,null,'2023-11-21','Administrator','2023-11-21','Administrator'
		
) 
AS Source
ON (Target.[PointTemplateTagAlarmStatusGuid] = Source.[PointTemplateTagAlarmStatusGuid])
WHEN MATCHED THEN
UPDATE SET target.[PointTemplateTagAlarmStatusGuid] = source.[PointTemplateTagAlarmStatusGuid]
		,target.[AlarmTestTemplateGuid] = source.[AlarmTestTemplateGuid]
		,target.[Acknowledged] = source.[Acknowledged]
		,target.[AcknowledgedTimestamp] = source.[AcknowledgedTimestamp]
		,target.[AcknowledgedBy] = source.[AcknowledgedBy]
		,target.[AcknowledgedComment] = source.[AcknowledgedComment]
		,target.[Silenced] = source.[Silenced]
		,target.[SilencedTimestamp] = source.[SilencedTimestamp]
		,target.[SilencedBy] = source.[SilencedBy]
		,target.[AlarmTestFailed] = source.[AlarmTestFailed]
		,target.[AlarmTestFailedTimestamp] = source.[AlarmTestFailedTimestamp]
		,target.[CreatedDate] = source.[CreatedDate]
		,target.[CreatedBy] = source.[CreatedBy]
		,target.[UpdatedDate] = source.[UpdatedDate]
		,target.[UpdatedBy] = source.[UpdatedBy]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([PointTemplateTagAlarmStatusGuid]
			,[AlarmTestTemplateGuid]
			,[Acknowledged]
			,[AcknowledgedTimestamp]
			,[AcknowledgedBy]
			,[AcknowledgedComment]
			,[Silenced]
			,[SilencedTimestamp]
			,[SilencedBy]
			,[AlarmTestFailed]
			,[AlarmTestFailedTimestamp]
			,[CreatedDate]
			,[CreatedBy]
			,[UpdatedDate]
			,[UpdatedBy])
	VALUES
			(Source.[PointTemplateTagAlarmStatusGuid]
			,Source.[AlarmTestTemplateGuid]
			,Source.[Acknowledged]
			,Source.[AcknowledgedTimestamp]
			,Source.[AcknowledgedBy]
			,Source.[AcknowledgedComment]
			,Source.[Silenced]
			,Source.[SilencedTimestamp]
			,Source.[SilencedBy]
			,Source.[AlarmTestFailed]
			,Source.[AlarmTestFailedTimestamp]
			,Source.[CreatedDate]
			,Source.[CreatedBy]
			,Source.[UpdatedDate]
			,Source.[UpdatedBy]);

DECLARE @StrapTableModuleToPointTemplateGuid UNIQUEIDENTIFIER = '3dec7303-494d-4e9c-afd7-32d4935e3aec'
DECLARE @VcfModuleToPointTemplateGuid UNIQUEIDENTIFIER = 'e9f45d1c-803e-490a-90b5-6607649054fa'
DECLARE @ShellCorrectionModuleToPointTemplateGuid UNIQUEIDENTIFIER = 'B15DAA3F-C060-419A-AC2D-22FA1AC4A352'
DECLARE @QuantityModuleToPointTemplateGuid UNIQUEIDENTIFIER = 'C14E3838-8017-498c-ABD0-FD4821CEC659'
DECLARE @FloatingRoofCorrectionModuleToPointTemplateGuid UNIQUEIDENTIFIER = 'EC21F254-CE43-49a1-93B0-22631E5E994D'
DECLARE @RateModuleLevelProductToPointTemplateGuid UNIQUEIDENTIFIER = '1B8AF8FA-1332-4256-9CE6-8303002AEDD6'
DECLARE @RateModuleTotalObservedToPointTemplateGuid UNIQUEIDENTIFIER = 'B9AB741B-1E22-4F2A-831A-B39387B56D7B'
DECLARE @RateModuleNetStandardToPointTemplateGuid UNIQUEIDENTIFIER = 'F897EE0A-3478-4427-A4EA-A4FD550A8093'
DECLARE @RateModuleGrossObservedToPointTemplateGuid UNIQUEIDENTIFIER =  'D3E5CA3F-DDD7-4CFD-923C-A3C198006EAF'
DECLARE @TanksCommandModuleToPointTemplateGuid UNIQUEIDENTIFIER = 'D13D739B-C766-40BA-96C3-D4E030729AE7'
DECLARE @TanksTransferModuleToPointTemplateGuid UNIQUEIDENTIFIER =  '7D15DAA1-6D4D-421B-AB6B-F63CFD73EBAF'
DECLARE @AvailableAndRemainingVolumeModuleToPointTemplateGuid UNIQUEIDENTIFIER = '1D177949-8F59-4A38-9511-622A3BA9ED84'
DECLARE @StandardTankCalculatorModuleToPointTemplateGuid UNIQUEIDENTIFIER = '9713E37B-9C7D-490b-A1EF-CFC345AAE99D'
DECLARE @LeakDetectionModuleToPointTemplateGuid UNIQUEIDENTIFIER = '999015D8-BF65-42C1-97CF-F9910FDD4238'


DECLARE @StrapTableModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>9eab1a9f-2aa2-4ec9-ac60-7231345a974a</TagGuid>
      <ModuleParameter>LevelProduct</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>257b0b99-b1f0-4fd2-bc76-348aee522a90</TagGuid>
      <ModuleParameter>LevelWater</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>1c9ea4b3-5460-450d-8971-d97cd0e43280</TagGuid>
      <ModuleParameter>LevelSolids</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>dcb0d63d-5f0a-4ab9-b454-7f124063ed47</TagGuid>
      <ModuleParameter>VolumeTotalObserved</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d5c390e3-ee24-41d8-96c0-43e5dfff8cc5</TagGuid>
      <ModuleParameter>VolumeWater</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>78100e99-0e39-4170-bb01-19f752f0d929</TagGuid>
      <ModuleParameter>VolumeStrapSolids</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules>
    <PropertyToModule>
      <PropertyGuid>f48f45a0-80b4-4cec-8dc1-4c49b1b72169</PropertyGuid>
      <PropertyName>StrapTable</PropertyName>
    </PropertyToModule>
    <PropertyToModule>
      <PropertyGuid>96fdc24a-2e74-4a3a-a20a-033659207a39</PropertyGuid>
      <PropertyName>Vessel</PropertyName>
    </PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'

DECLARE @VcfModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>8624008f-d28c-496d-8578-7227e329e493</TagGuid>
      <ModuleParameter>TemperatureProduct</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d350feab-229a-4808-a7a1-76e552501b47</TagGuid>
      <ModuleParameter>TemperatureDensity</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>c58c9b1c-6471-474f-a2b4-6d9d2a5b5e7b</TagGuid>
      <ModuleParameter>TemperatureVapor</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>8f82abfb-8ed8-4a4b-9424-672c8e74752a</TagGuid>
      <ModuleParameter>DensityStandard</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>5c6f08cc-1cef-4af6-b25c-cd12f3c82fb7</TagGuid>
      <ModuleParameter>DensityObserved</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>a3d5835e-5f79-4110-8ba9-b868949e6eb9</TagGuid>
      <ModuleParameter>PressureVapor</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>9127882d-a34d-4465-a338-db9bc7cf5d02</TagGuid>
      <ModuleParameter>VolumeCorrectionForTemperature</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>46c7073b-546c-4d31-b1bf-642df6cc74ac</TagGuid>
      <ModuleParameter>VolumeCorrectionForPressure</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>58adee26-8dec-47d8-bb44-870dc5d8cddf</TagGuid>
      <ModuleParameter>VolumeCorrectionForTemperatureandPressure</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>72269896-29d3-4082-856d-812e8bd90319</TagGuid>
      <ModuleParameter>VolumeCorrectionFactor</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>D83854E0-41F4-474A-BDF4-21D2172065A4</TagGuid>
      <ModuleParameter>VolumeCorrectionFactorUnrounded</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>70ec0770-89b6-4ef1-847d-c97ec459e988</TagGuid>
      <ModuleParameter>APICorrectionError</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>7f820d6e-d913-4258-8218-d0a68a8c4590</TagGuid>
      <ModuleParameter>DensityObservedInAir</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>78b4749f-f02b-489c-9b0c-46c2f9e02116</TagGuid>
      <ModuleParameter>DensityStandardInAir</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>07A95F08-2794-480e-92BD-0FF62CD8F7F2</TagGuid>
      <ModuleParameter>DensityGauge</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules>
    <PropertyToModule>
      <PropertyGuid>8529fc7f-7d00-4344-a968-58273c7ee6d7</PropertyGuid>
      <PropertyName>VcfSettings</PropertyName>
    </PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'

DECLARE @ShellCorrectionModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>af72ed89-fa23-446d-a551-16a915c8e0e9</TagGuid>
      <ModuleParameter>TemperatureAmbient</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>8624008f-d28c-496d-8578-7227e329e493</TagGuid>
      <ModuleParameter>TemperatureProduct</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>526a0f15-ef35-45bc-8f3d-cd2df565a649</TagGuid>
      <ModuleParameter>TankShellCorrection</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules>
    <PropertyToModule>
      <PropertyGuid>8529fc7f-7d00-4344-a968-58273c7ee6d7</PropertyGuid>
      <PropertyName>VcfSettings</PropertyName>
    </PropertyToModule>
    <PropertyToModule>
      <PropertyGuid>96fdc24a-2e74-4a3a-a20a-033659207a39</PropertyGuid>
      <PropertyName>Vessel</PropertyName>
    </PropertyToModule>
    <PropertyToModule>
      <PropertyGuid>f48f45a0-80b4-4cec-8dc1-4c49b1b72169</PropertyGuid>
      <PropertyName>StrapTable</PropertyName>
    </PropertyToModule>
    <PropertyToModule>
      <PropertyGuid>e51c7a94-96cd-4e2b-ad1d-12e62f916998</PropertyGuid>
      <PropertyName>QuantitySettings</PropertyName>
    </PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'

DECLARE @FloatingRoofCorrectionModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>8624008f-d28c-496d-8578-7227e329e493</TagGuid>
      <ModuleParameter>TemperatureProduct</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>5c6f08cc-1cef-4af6-b25c-cd12f3c82fb7</TagGuid>
      <ModuleParameter>DensityObserved</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>7f820d6e-d913-4258-8218-d0a68a8c4590</TagGuid>
      <ModuleParameter>DensityObservedInAir</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d4bd1ce3-d5e4-45b0-98a0-f8e5240e2a64</TagGuid>
      <ModuleParameter>Mass</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>9eab1a9f-2aa2-4ec9-ac60-7231345a974a</TagGuid>
      <ModuleParameter>LevelProduct</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>72269896-29d3-4082-856d-812e8bd90319</TagGuid>
      <ModuleParameter>VolumeCorrectionFactor</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>dbf87c9c-65db-41fe-aab0-37129382866f</TagGuid>
      <ModuleParameter>CriticalZone</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>9173ad63-65c1-4c43-bb5a-054ffeabf1fe</TagGuid>
      <ModuleParameter>VolumeRoofCorrection</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules>
    <PropertyToModule>
      <PropertyGuid>f48f45a0-80b4-4cec-8dc1-4c49b1b72169</PropertyGuid>
      <PropertyName>StrapTable</PropertyName>
    </PropertyToModule>
    <PropertyToModule>
      <PropertyGuid>e51c7a94-96cd-4e2b-ad1d-12e62f916998</PropertyGuid>
      <PropertyName>QuantitySettings</PropertyName>
    </PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'

DECLARE @QuantitiesModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>9173ad63-65c1-4c43-bb5a-054ffeabf1fe</TagGuid>
      <ModuleParameter>VolumeRoofCorrection</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>dcb0d63d-5f0a-4ab9-b454-7f124063ed47</TagGuid>
      <ModuleParameter>VolumeTotalObserved</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d5c390e3-ee24-41d8-96c0-43e5dfff8cc5</TagGuid>
      <ModuleParameter>VolumeWater</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>78100e99-0e39-4170-bb01-19f752f0d929</TagGuid>
      <ModuleParameter>VolumeStrapSolids</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d723f705-01f6-4c46-92ff-bf46ab9a4c62</TagGuid>
      <ModuleParameter>PercentBSW</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>72269896-29d3-4082-856d-812e8bd90319</TagGuid>
      <ModuleParameter>VolumeCorrectionFactor</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d83854e0-41f4-474a-bdf4-21d2172065a4</TagGuid>
      <ModuleParameter>VolumeCorrectionFactorUnrounded</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>8f82abfb-8ed8-4a4b-9424-672c8e74752a</TagGuid>
      <ModuleParameter>DensityStandard</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>5c6f08cc-1cef-4af6-b25c-cd12f3c82fb7</TagGuid>
      <ModuleParameter>DensityObserved</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>526a0f15-ef35-45bc-8f3d-cd2df565a649</TagGuid>
      <ModuleParameter>TankShellCorrection</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>7f820d6e-d913-4258-8218-d0a68a8c4590</TagGuid>
      <ModuleParameter>DensityObservedInAir</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>78b4749f-f02b-489c-9b0c-46c2f9e02116</TagGuid>
      <ModuleParameter>DensityStandardInAir</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>9bfebbe8-bf75-430b-8f79-0de0aa6dd430</TagGuid>
      <ModuleParameter>DensityVapor</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>c58c9b1c-6471-474f-a2b4-6d9d2a5b5e7b</TagGuid>
      <ModuleParameter>TemperatureVapor</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>a3d5835e-5f79-4110-8ba9-b868949e6eb9</TagGuid>
      <ModuleParameter>PressureVapor</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>c48d535a-2082-4c7b-ac91-93d17d89add8</TagGuid>
      <ModuleParameter>VolumeBottom</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>2a80e1ce-933f-44bd-b0ed-c3c2861ce89d</TagGuid>
      <ModuleParameter>VolumeGrossObserved</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>9a467cd0-6f77-4541-bef3-c2c7e8879f05</TagGuid>
      <ModuleParameter>VolumeNetStandard</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>34cf4d6f-b832-4c8d-82f4-9ae591ca2740</TagGuid>
      <ModuleParameter>VolumeNetStandardUnrounded</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d4bd1ce3-d5e4-45b0-98a0-f8e5240e2a64</TagGuid>
      <ModuleParameter>Mass</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>f640abc2-f3b7-471a-834a-d231ace7dc2a</TagGuid>
      <ModuleParameter>WeightGrossStandard</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>76699ab2-813b-45f9-9f85-3de09f22d6dd</TagGuid>
      <ModuleParameter>VolumeGrossStandard</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>ef176082-14e0-4a30-8fcc-53465f70e897</TagGuid>
      <ModuleParameter>WeightNetStandard</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>13ee61b5-5420-4cc1-9a6c-56db3022423e</TagGuid>
      <ModuleParameter>VolumeBSW</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>0434b936-4328-4bce-a4de-9befc2307437</TagGuid>
      <ModuleParameter>VolumeTotalCalculated</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>ae0ef9ec-9d71-4e57-929e-4ed668026383</TagGuid>
      <ModuleParameter>VolumeVaporNet</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>0b5fb637-24bf-428a-9152-e327719f6b5e</TagGuid>
      <ModuleParameter>MassVapor</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules>
    <PropertyToModule>
      <PropertyGuid>e51c7a94-96cd-4e2b-ad1d-12e62f916998</PropertyGuid>
      <PropertyName>QuantitySettings</PropertyName>
    </PropertyToModule>
    <PropertyToModule>
      <PropertyGuid>8529fc7f-7d00-4344-a968-58273c7ee6d7</PropertyGuid>
      <PropertyName>VcfSettings</PropertyName>
    </PropertyToModule>
    <PropertyToModule>
      <PropertyGuid>96fdc24a-2e74-4a3a-a20a-033659207a39</PropertyGuid>
      <PropertyName>Vessel</PropertyName>
    </PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'

DECLARE @LevelRateModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>9eab1a9f-2aa2-4ec9-ac60-7231345a974a</TagGuid>
      <ModuleParameter>Value</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>70adfa50-ccca-4abc-8055-5889f4433e26</TagGuid>
      <ModuleParameter>Rate</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules>
    <PropertyToModule>
      <PropertyGuid>15443351-1677-43de-8775-911d0885175b</PropertyGuid>
      <PropertyName>Settings</PropertyName>
    </PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'

DECLARE @VolumeTotalObservedRateModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>dcb0d63d-5f0a-4ab9-b454-7f124063ed47</TagGuid>
      <ModuleParameter>Value</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>f7b61b07-7364-4dbf-82c8-6edc1e7c6e21</TagGuid>
      <ModuleParameter>Rate</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules>
    <PropertyToModule>
      <PropertyGuid>7fcd093e-3d52-46aa-8f4b-d68e21328423</PropertyGuid>
      <PropertyName>Settings</PropertyName>
    </PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'

DECLARE @VolumeNetStandardRateModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>9a467cd0-6f77-4541-bef3-c2c7e8879f05</TagGuid>
      <ModuleParameter>Value</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>2d928f24-9b54-40d5-b00b-d19d87069d74</TagGuid>
      <ModuleParameter>Rate</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules>
    <PropertyToModule>
      <PropertyGuid>41eebef8-7020-47f2-a95a-3e084f6ce3dc</PropertyGuid>
      <PropertyName>Settings</PropertyName>
    </PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'

DECLARE @VolumeGrossObservedRateModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>2a80e1ce-933f-44bd-b0ed-c3c2861ce89d</TagGuid>
      <ModuleParameter>Value</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>66bdc1eb-98f8-4179-b54f-e9cbca9d8de0</TagGuid>
      <ModuleParameter>Rate</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules>
    <PropertyToModule>
      <PropertyGuid>90046419-3720-43bb-bda6-bd74ee2a87e3</PropertyGuid>
      <PropertyName>Settings</PropertyName>
    </PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'

DECLARE @AvailableAndRemainingVolumeModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>79c8b5a4-30e7-4a4c-81be-38f82eb50bf3</TagGuid>
      <ModuleParameter>LevelMinOpLimit</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>11469d43-5c8b-492e-b166-272abfe7976a</TagGuid>
      <ModuleParameter>LevelMaxOpLimit</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>dcb0d63d-5f0a-4ab9-b454-7f124063ed47</TagGuid>
      <ModuleParameter>VolumeTotalObserved</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>9173ad63-65c1-4c43-bb5a-054ffeabf1fe</TagGuid>
      <ModuleParameter>VolumeRoofCorrection</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>8624008f-d28c-496d-8578-7227e329e493</TagGuid>
      <ModuleParameter>TemperatureProduct</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>5c6f08cc-1cef-4af6-b25c-cd12f3c82fb7</TagGuid>
      <ModuleParameter>DensityObserved</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>78B4749F-F02B-489c-9B0C-46C2F9E02116</TagGuid>
      <ModuleParameter>DensityProductStandardinAir</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d4bd1ce3-d5e4-45b0-98a0-f8e5240e2a64</TagGuid>
      <ModuleParameter>Mass</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>72269896-29d3-4082-856d-812e8bd90319</TagGuid>
      <ModuleParameter>VolumeCorrectionFactor</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>c48d535a-2082-4c7b-ac91-93d17d89add8</TagGuid>
      <ModuleParameter>VolumeBottom</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>dbf87c9c-65db-41fe-aab0-37129382866f</TagGuid>
      <ModuleParameter>CriticalZone</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d5c390e3-ee24-41d8-96c0-43e5dfff8cc5</TagGuid>
      <ModuleParameter>VolumeWater</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>78100e99-0e39-4170-bb01-19f752f0d929</TagGuid>
      <ModuleParameter>VolumeStrapSolids</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d723f705-01f6-4c46-92ff-bf46ab9a4c62</TagGuid>
      <ModuleParameter>PercentBSW</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>526a0f15-ef35-45bc-8f3d-cd2df565a649</TagGuid>
      <ModuleParameter>TankShellCorrection</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>2a80e1ce-933f-44bd-b0ed-c3c2861ce89d</TagGuid>
      <ModuleParameter>VolumeGrossObserved</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>9a467cd0-6f77-4541-bef3-c2c7e8879f05</TagGuid>
      <ModuleParameter>VolumeNetStandard</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>ea90b57c-e223-4041-95c0-8bc15a097755</TagGuid>
      <ModuleParameter>VolumeGOVAvailable</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>816a00c8-fcd7-4d79-a17c-bd8c8f0ad2dc</TagGuid>
      <ModuleParameter>VolumeNSVAvailable</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>abafccd1-1480-47a4-81af-2d86413dd27d</TagGuid>
      <ModuleParameter>VolumeGOVRemaining</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>8f35b5d4-ca96-47fa-8a76-ad99326b9d19</TagGuid>
      <ModuleParameter>VolumeNSVRemaining</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules />
</ModuleToPointTemplateData>'

DECLARE @TransferModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>9eab1a9f-2aa2-4ec9-ac60-7231345a974a</TagGuid>
      <ModuleParameter>LevelProduct</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>2a80e1ce-933f-44bd-b0ed-c3c2861ce89d</TagGuid>
      <ModuleParameter>VolumeGrossObserved</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>70adfa50-ccca-4abc-8055-5889f4433e26</TagGuid>
      <ModuleParameter>LevelProductRate</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>66bdc1eb-98f8-4179-b54f-e9cbca9d8de0</TagGuid>
      <ModuleParameter>VolumeGrossObservedRate</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>2d928f24-9b54-40d5-b00b-d19d87069d74</TagGuid>
      <ModuleParameter>VolumeNetStandardRate</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>9a467cd0-6f77-4541-bef3-c2c7e8879f05</TagGuid>
      <ModuleParameter>VolumeNetStandard</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>D5C390E3-EE24-41D8-96C0-43E5DFFF8CC5</TagGuid>
      <ModuleParameter>VolumeWater</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>b24e6a16-ab76-4980-a648-07a724d84a74</TagGuid>
      <ModuleParameter>TransferMode</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>7a451a61-e2e6-480e-ae85-609e7bc2a57f</TagGuid>
      <ModuleParameter>TransferStatus</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>cf589e1e-5bbe-49f5-8381-3160709e2889</TagGuid>
      <ModuleParameter>TransferTarget</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>8624008f-d28c-496d-8578-7227e329e493</TagGuid>
      <ModuleParameter>TemperatureProduct</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>5c6f08cc-1cef-4af6-b25c-cd12f3c82fb7</TagGuid>
      <ModuleParameter>DensityObserved</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>7f820d6e-d913-4258-8218-d0a68a8c4590</TagGuid>
      <ModuleParameter>DensityObservedInAir</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d4bd1ce3-d5e4-45b0-98a0-f8e5240e2a64</TagGuid>
      <ModuleParameter>Mass</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>72269896-29d3-4082-856d-812e8bd90319</TagGuid>
      <ModuleParameter>VolumeCorrectionFactor</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>c48d535a-2082-4c7b-ac91-93d17d89add8</TagGuid>
      <ModuleParameter>VolumeBottom</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>526a0f15-ef35-45bc-8f3d-cd2df565a649</TagGuid>
      <ModuleParameter>TankShellCorrection</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d723f705-01f6-4c46-92ff-bf46ab9a4c62</TagGuid>
      <ModuleParameter>PercentBSW</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>ea90b57c-e223-4041-95c0-8bc15a097755</TagGuid>
      <ModuleParameter>VolumeGOVAvailable</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>abafccd1-1480-47a4-81af-2d86413dd27d</TagGuid>
      <ModuleParameter>VolumeGOVRemaining</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>816a00c8-fcd7-4d79-a17c-bd8c8f0ad2dc</TagGuid>
      <ModuleParameter>VolumeNSVAvailable</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>8f35b5d4-ca96-47fa-8a76-ad99326b9d19</TagGuid>
      <ModuleParameter>VolumeNSVRemaining</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>12f92cbc-bea9-472d-87b2-34d2a838647c</TagGuid>
      <ModuleParameter>TransferStartLevelProduct</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>dc7bfbca-4f68-46eb-92b4-3921b6e13019</TagGuid>
      <ModuleParameter>TransferStartGOV</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>8678ea00-e579-4a04-9dc4-f655731dce3c</TagGuid>
      <ModuleParameter>TransferStartNSV</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>3D8C66F7-E4B7-4CD4-91B1-65C8FE050EBD</TagGuid>
      <ModuleParameter>TransferStartVolumeWater</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>f270261c-c376-4617-98fa-37c67a4c1019</TagGuid>
      <ModuleParameter>TransferStartVolume</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>37a589e6-4230-4a9f-8044-aa66c7bed7a7</TagGuid>
      <ModuleParameter>TransferTimeRemaining</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>c34bc94b-f8e8-41ed-80e4-5feb10094785</TagGuid>
      <ModuleParameter>TransferTimeCompletion</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>ee65d3e3-818f-4304-846c-ab711471185c</TagGuid>
      <ModuleParameter>TransferredGOV</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>b7043057-e2c9-4bd3-848c-04fd0cd6e0a7</TagGuid>
      <ModuleParameter>TransferredNSV</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>06DA0DAF-1227-4E3B-85CA-689A95101060</TagGuid>
      <ModuleParameter>TransferredVolumeWater</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>b9361ff1-4d5f-44c8-aa36-8302f26e2bee</TagGuid>
      <ModuleParameter>TransferredVolume</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>c2400fdd-55bb-4e43-abb5-7d87b510513a</TagGuid>
      <ModuleParameter>TankTransferDiscreteAlarm</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>99164989-a1db-4c83-a834-01396b8d589e</TagGuid>
      <ModuleParameter>TankCommand</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>11469d43-5c8b-492e-b166-272abfe7976a</TagGuid>
      <ModuleParameter>LevelMaxOpLimit</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>79c8b5a4-30e7-4a4c-81be-38f82eb50bf3</TagGuid>
      <ModuleParameter>LevelMinOpLimit</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>F60377C7-DD21-4ECA-B22C-BE2F6950C85E</TagGuid>
      <ModuleParameter>TransferStartTime</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>349FAEA0-766A-4C20-BF1F-7CB85E7BE1FC</TagGuid>
      <ModuleParameter>TransferStopTime</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>64710B23-C502-4513-8C7C-8B7812ABA684</TagGuid>
      <ModuleParameter>TransferLevelTarget</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>86A99CD8-0235-4CC5-BDA4-F5E4CBD15894</TagGuid>
      <ModuleParameter>TransferVolumeTarget</ModuleParameter>
    </TagToModule>

 </TagToModules>
  <PropertyToModules>
    <PropertyToModule>
      <PropertyGuid>b2da91e2-cd00-44fa-ab8d-dda06a12b7ba</PropertyGuid>
      <PropertyName>TankTransferSettings</PropertyName>
    </PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'

DECLARE @TankCommandModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>99164989-a1db-4c83-a834-01396b8d589e</TagGuid>
      <ModuleParameter>TankCommand</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>f7b61b07-7364-4dbf-82c8-6edc1e7c6e21</TagGuid>
      <ModuleParameter>VolumeTotalObservedRate</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>9eab1a9f-2aa2-4ec9-ac60-7231345a974a</TagGuid>
      <ModuleParameter>LevelProduct</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>edd65b84-474f-4cd9-b169-42517668338c</TagGuid>
      <ModuleParameter>TankStatus</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>3e0b375c-f090-430a-bc16-a4c9883d0f13</TagGuid>
      <ModuleParameter>TankModeDiscreteAlarm</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>b9e03854-eddc-4165-8a11-6fd56b79e988</TagGuid>
      <ModuleParameter>LevelProductStop</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>A7B30ADC-FBF2-4F35-94A6-ED75B9E5E062</TagGuid>
      <ModuleParameter>LevelProductMovement</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules>
    <PropertyToModule>
      <PropertyGuid>5150a1f0-6e7b-480b-9ae2-f113669b1955</PropertyGuid>
      <PropertyName>TankCommandSettings</PropertyName>
    </PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'

DECLARE @StandardTankCalculatorModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>9eab1a9f-2aa2-4ec9-ac60-7231345a974a</TagGuid>
      <ModuleParameter>LevelProduct</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>257b0b99-b1f0-4fd2-bc76-348aee522a90</TagGuid>
      <ModuleParameter>LevelWater</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>1c9ea4b3-5460-450d-8971-d97cd0e43280</TagGuid>
      <ModuleParameter>LevelSolids</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>dcb0d63d-5f0a-4ab9-b454-7f124063ed47</TagGuid>
      <ModuleParameter>VolumeTotalObserved</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d5c390e3-ee24-41d8-96c0-43e5dfff8cc5</TagGuid>
      <ModuleParameter>VolumeWater</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>78100e99-0e39-4170-bb01-19f752f0d929</TagGuid>
      <ModuleParameter>VolumeSolids</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>8624008f-d28c-496d-8578-7227e329e493</TagGuid>
      <ModuleParameter>TemperatureProduct</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d350feab-229a-4808-a7a1-76e552501b47</TagGuid>
      <ModuleParameter>TemperatureDensity</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>c58c9b1c-6471-474f-a2b4-6d9d2a5b5e7b</TagGuid>
      <ModuleParameter>TemperatureVapor</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>af72ed89-fa23-446d-a551-16a915c8e0e9</TagGuid>
      <ModuleParameter>TemperatureAmbient</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>a3d5835e-5f79-4110-8ba9-b868949e6eb9</TagGuid>
      <ModuleParameter>PressureVapor</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d723f705-01f6-4c46-92ff-bf46ab9a4c62</TagGuid>
      <ModuleParameter>PercentBSW</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>8f82abfb-8ed8-4a4b-9424-672c8e74752a</TagGuid>
      <ModuleParameter>DensityProductStandard</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>5c6f08cc-1cef-4af6-b25c-cd12f3c82fb7</TagGuid>
      <ModuleParameter>DensityProductObserved</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>9bfebbe8-bf75-430b-8f79-0de0aa6dd430</TagGuid>
      <ModuleParameter>DensityVapor</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>526a0f15-ef35-45bc-8f3d-cd2df565a649</TagGuid>
      <ModuleParameter>TankShellCorrection</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>72269896-29d3-4082-856d-812e8bd90319</TagGuid>
      <ModuleParameter>VolumeCorrectionFactor</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>c48d535a-2082-4c7b-ac91-93d17d89add8</TagGuid>
      <ModuleParameter>VolumeBottoms</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>2a80e1ce-933f-44bd-b0ed-c3c2861ce89d</TagGuid>
      <ModuleParameter>VolumeGrossObserved</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>ea90b57c-e223-4041-95c0-8bc15a097755</TagGuid>
      <ModuleParameter>VolumeGrossObservedAvailable</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>abafccd1-1480-47a4-81af-2d86413dd27d</TagGuid>
      <ModuleParameter>VolumeGrossObservedRemaining</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>9a467cd0-6f77-4541-bef3-c2c7e8879f05</TagGuid>
      <ModuleParameter>VolumeNetStandard</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>816a00c8-fcd7-4d79-a17c-bd8c8f0ad2dc</TagGuid>
      <ModuleParameter>VolumeNetStandardAvailable</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>8f35b5d4-ca96-47fa-8a76-ad99326b9d19</TagGuid>
      <ModuleParameter>VolumeNetStandardRemaining</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>9173ad63-65c1-4c43-bb5a-054ffeabf1fe</TagGuid>
      <ModuleParameter>VolumeRoofCorrection</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d4bd1ce3-d5e4-45b0-98a0-f8e5240e2a64</TagGuid>
      <ModuleParameter>MassLiquid</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>0b5fb637-24bf-428a-9152-e327719f6b5e</TagGuid>
      <ModuleParameter>MassVapor</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>9127882d-a34d-4465-a338-db9bc7cf5d02</TagGuid>
      <ModuleParameter>VolumeCorrectionForTemperature</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>46c7073b-546c-4d31-b1bf-642df6cc74ac</TagGuid>
      <ModuleParameter>VolumeCorrectionForPressure</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>58adee26-8dec-47d8-bb44-870dc5d8cddf</TagGuid>
      <ModuleParameter>VolumeCorrectionForTemperatureandPressure</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>70ec0770-89b6-4ef1-847d-c97ec459e988</TagGuid>
      <ModuleParameter>APICorrectionError</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>7f820d6e-d913-4258-8218-d0a68a8c4590</TagGuid>
      <ModuleParameter>DensityProductInAir</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>78b4749f-f02b-489c-9b0c-46c2f9e02116</TagGuid>
      <ModuleParameter>DensityProductStandardinAir</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>dbf87c9c-65db-41fe-aab0-37129382866f</TagGuid>
      <ModuleParameter>RoofCriticalZone</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>f640abc2-f3b7-471a-834a-d231ace7dc2a</TagGuid>
      <ModuleParameter>WeightGrossStandard</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>ef176082-14e0-4a30-8fcc-53465f70e897</TagGuid>
      <ModuleParameter>WeightNetStandard</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>76699ab2-813b-45f9-9f85-3de09f22d6dd</TagGuid>
      <ModuleParameter>VolumeGrossStandard</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>13ee61b5-5420-4cc1-9a6c-56db3022423e</TagGuid>
      <ModuleParameter>VolumeBSW</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>0434b936-4328-4bce-a4de-9befc2307437</TagGuid>
      <ModuleParameter>VolumeTotalCalculated</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>ae0ef9ec-9d71-4e57-929e-4ed668026383</TagGuid>
      <ModuleParameter>VolumeVaporNet</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>79c8b5a4-30e7-4a4c-81be-38f82eb50bf3</TagGuid>
      <ModuleParameter>LevelProductMinOpLimit</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>11469d43-5c8b-492e-b166-272abfe7976a</TagGuid>
      <ModuleParameter>LevelProductMaxOpLimit</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>07a95f08-2794-480e-92bd-0ff62cd8f7f2</TagGuid>
      <ModuleParameter>DensityGauge</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>d83854e0-41f4-474a-bdf4-21d2172065a4</TagGuid>
      <ModuleParameter>VolumeCorrectionFactorUnrounded</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>34cf4d6f-b832-4c8d-82f4-9ae591ca2740</TagGuid>
      <ModuleParameter>VolumeNetStandardUnrounded</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules />
</ModuleToPointTemplateData>'

DECLARE @LeakDetectionModuleToPointTemplateData NVARCHAR(MAX) =
'<ModuleToPointTemplateData>
  <TagToModules>
    <TagToModule>
      <TagGuid>34CF4D6F-B832-4C8D-82F4-9AE591CA2740</TagGuid>
      <ModuleParameter>VolumeNetStandardUnrounded</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>D83854E0-41F4-474A-BDF4-21D2172065A4</TagGuid>
      <ModuleParameter>VolumeCorrectionFactorUnrounded</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>F27E3FEB-E180-4F6A-81F9-11C09FD17812</TagGuid>
      <ModuleParameter>PressureBottom</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>5F541EB7-C6A3-477B-8E8E-3C8A3B9F53B4</TagGuid>
      <ModuleParameter>LeakRate</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>1B5DA8D0-8880-41A4-AC0C-D915F2BF0593</TagGuid>
      <ModuleParameter>LeakRateHighAlarm</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>8228F563-D0B7-4445-A7BF-F123F2504EB6</TagGuid>
      <ModuleParameter>LeakRateHighLimit</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>1FA8E5CC-B11C-46A2-B40F-C0907B64901E</TagGuid>
      <ModuleParameter>LeakDetectionDiscreteAlarm</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>45F7293C-1907-42C9-A929-9BC246727117</TagGuid>
      <ModuleParameter>LeakDetectionDataInsufficientLimit</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>733963D9-1FE7-45CB-B362-00B2D35A95AD</TagGuid>
      <ModuleParameter>LeakDetectionAlarm</ModuleParameter>
    </TagToModule>
    <TagToModule>
      <TagGuid>18028EDA-1E50-4090-89A2-E99EA25EA221</TagGuid>
      <ModuleParameter>LeakDetectionDataLastRunTime</ModuleParameter>
    </TagToModule>
	 <TagToModule>
      <TagGuid>9a467cd0-6f77-4541-bef3-c2c7e8879f05</TagGuid>
      <ModuleParameter>VolumeNetStandard</ModuleParameter>
    </TagToModule>
	<TagToModule>
      <TagGuid>9eab1a9f-2aa2-4ec9-ac60-7231345a974a</TagGuid>
      <ModuleParameter>LevelProduct</ModuleParameter>
    </TagToModule>
	<TagToModule>
      <TagGuid>257b0b99-b1f0-4fd2-bc76-348aee522a90</TagGuid>
      <ModuleParameter>LevelWater</ModuleParameter>
    </TagToModule>
	<TagToModule>
      <TagGuid>8624008f-d28c-496d-8578-7227e329e493</TagGuid>
      <ModuleParameter>TemperatureProduct</ModuleParameter>
    </TagToModule>
  </TagToModules>
  <PropertyToModules>
    <PropertyToModule>
      <PropertyGuid>9043DA8B-C2D1-4744-976B-524DD600E036</PropertyGuid>
      <PropertyName>LeakDetectionSettings</PropertyName>
    </PropertyToModule>
  </PropertyToModules>
</ModuleToPointTemplateData>'

--Create mapping of module to Point Template

MERGE map.tblModuleToPointTemplate AS Target
USING 
(  SELECT 
'Strap Table' as [ID],
1 as [Order],
@StrapTableModuleToPointTemplateData AS [ModuleToPointTemplateData],
@StrapTableModuleToPointTemplateGuid as [ModuleToPointTemplateGuid],
@PointTemplateGuid as [PointTemplateGuid] ,
@StrapTableModuleGuid as [ModuleGuid],
'2015-02-04' as [CreatedDate] ,
'Administrator' as [CreatedBy],
'2015-02-04' as [UpdatedDate],
'Administrator' as [UpdatedBy]
UNION ALL
SELECT 'Volume Correction',2, @VcfModuleToPointTemplateData, @VcfModuleToPointTemplateGuid,@PointTemplateGuid,@VcfModuleGuid,'2015-02-04','Administrator','2015-02-04','Administrator'
UNION ALL
SELECT 'Shell Correction',3,@ShellCorrectionModuleToPointTemplateData, @ShellCorrectionModuleToPointTemplateGuid,@PointTemplateGuid,@ShellCorrectionModuleGuid,'2016-05-31','Administrator','2016-05-31','Administrator'
UNION ALL
SELECT 'Floating Roof Correction',4, @FloatingRoofCorrectionModuleToPointTemplateData,  @FloatingRoofCorrectionModuleToPointTemplateGuid,@PointTemplateGuid,@RoofCorrectionModuleGuid,'2015-02-04','Administrator','2015-02-04','Administrator'
UNION ALL
SELECT 'Quantities',5, @QuantitiesModuleToPointTemplateData ,@QuantityModuleToPointTemplateGuid,@PointTemplateGuid,@QuantityModuleGuid,'2015-02-04','Administrator','2015-02-04','Administrator'
UNION ALL
SELECT 'Rate Level Product',6, @LevelRateModuleToPointTemplateData, @RateModuleLevelProductToPointTemplateGuid, @PointTemplateGuid, @RateModuleGuid,'2017-08-11','Administrator','2017-08-11','Administrator'
UNION ALL
SELECT 'Rate Volume Total Observed',7, @VolumeTotalObservedRateModuleToPointTemplateData, @RateModuleTotalObservedToPointTemplateGuid, @PointTemplateGuid, @RateModuleGuid,'2017-08-30','Administrator','2017-08-30','Administrator'
UNION ALL
SELECT 'Rate Volume Net Standard',8, @VolumeNetStandardRateModuleToPointTemplateData, @RateModuleNetStandardToPointTemplateGuid, @PointTemplateGuid, @RateModuleGuid,'2017-08-11','Administrator','2017-08-11','Administrator'
UNION ALL
SELECT 'Rate Volume Gross Observed',9, @VolumeGrossObservedRateModuleToPointTemplateData, @RateModuleGrossObservedToPointTemplateGuid, @PointTemplateGuid, @RateModuleGuid,'2017-08-30','Administrator','2017-08-30','Administrator'
UNION ALL
SELECT 'Available And Remaining Volume',10, @AvailableAndRemainingVolumeModuleToPointTemplateData, @AvailableAndRemainingVolumeModuleToPointTemplateGuid,@PointTemplateGuid,@AvailableAndRemainingVolumeModuleGuid,'2015-02-04','Administrator','2015-02-04','Administrator'
UNION ALL
SELECT 'Tank Transfer',11, @TransferModuleToPointTemplateData, @TanksTransferModuleToPointTemplateGuid,@PointTemplateGuid,@TankTransferModuleGuid,'2015-02-04','Administrator','2015-02-04','Administrator'
UNION ALL
SELECT 'Tank Commands',12, @TankCommandModuleToPointTemplateData, @TanksCommandModuleToPointTemplateGuid,@PointTemplateGuid,@TankCommandModuleGuid,'2015-02-04','Administrator','2015-02-04','Administrator'
UNION ALL
SELECT 'Standard Tank Calculator',13, @StandardTankCalculatorModuleToPointTemplateData, @StandardTankCalculatorModuleToPointTemplateGuid,@PointTemplateGuid,@StandardTankCalculatorModuleGuid,'2015-02-04','Administrator','2015-02-04','Administrator'
UNION ALL
SELECT 'Leak Detection',14, @LeakDetectionModuleToPointTemplateData, @LeakDetectionModuleToPointTemplateGuid, @PointTemplateGuid, @LeakDetectionModuleGuid, '2023-04-05', 'Administrator', '2023-04-05', 'Administrator'

) 
AS Source
ON (Target.[ID] = Source.[ID] AND Target.[ModuleToPointTemplateGuid] = Source.[ModuleToPointTemplateGuid] AND Target.[PointTemplateGuid] = Source.[PointTemplateGuid])
WHEN MATCHED THEN
	UPDATE SET 
						target.[ID] = source.[ID],
						target.[Order] = source.[Order],
						target.[ModuleToPointTemplateData] = source.[ModuleToPointTemplateData],
						target.[ModuleToPointTemplateGuid] = source.[ModuleToPointTemplateGuid],
						target.[PointTemplateGuid] = source.[PointTemplateGuid],
						target.[ModuleGuid] = source.[ModuleGuid],
						target.[CreatedDate] = source.[CreatedDate],
						target.[CreatedBy] = source.[CreatedBy],
						target.[UpdatedDate] = source.[UpdatedDate],
						target.[UpdatedBy] = source.[UpdatedBy]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[Order],[ModuleToPointTemplateData],[ModuleToPointTemplateGuid],[PointTemplateGuid],[ModuleGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
	VALUES (source.[ID],source.[Order],source.[ModuleToPointTemplateData],source.[ModuleToPointTemplateGuid],source.[PointTemplateGuid],source.[ModuleGuid],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy]);


DECLARE @StrapTablePointTemplatePropertyGuid UNIQUEIDENTIFIER = 'F48F45A0-80B4-4CEC-8DC1-4C49B1B72169'

--Create Strap Table Property
--Note : Strap Table Value is xml serialization of FMBusinessObjects StrapTable with 4 StrapTableEntry and Roof Settings
MERGE dbo.tblPointTemplateProperty AS Target
USING 
(  SELECT 'Strap Table' as [ID],
'FMBusinessObjects.DataObjects.StrapTable' as [ValueType],
'<StrapTable xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <SelectedTableForStrap>0</SelectedTableForStrap>
  <SelectedTableForWaterVolume>0</SelectedTableForWaterVolume>
  <SelectedTableForSolidsVolume>0</SelectedTableForSolidsVolume>
  <StrapTables>
	<IndividualStrapTable>
		<table>
			<StrapTableEntry>
			<Level>0</Level>
			<Volume>0</Volume>
			</StrapTableEntry>
			<StrapTableEntry>
			<Level>10</Level>
			<Volume>2500</Volume>
			</StrapTableEntry>
			<StrapTableEntry>
			<Level>20</Level>
			<Volume>5000</Volume>
			</StrapTableEntry>
			<StrapTableEntry>
			<Level>40</Level>
			<Volume>10000</Volume>
			</StrapTableEntry>
		</table>
		<StrapTableDescription>Strap Table 1</StrapTableDescription>
		<StrapDensity>
			<EngineeringUnitsType>FmuDensity</EngineeringUnitsType>
			<Value>60</Value>
		</StrapDensity>
		<StrapTemperature>
			<EngineeringUnitsType>FmuTemp</EngineeringUnitsType>
			<Value>60</Value>
		</StrapTemperature>
		<TankShellReferenceTemperature>
			<EngineeringUnitsType>FmuTemp</EngineeringUnitsType>
			<Value>60</Value>
		</TankShellReferenceTemperature>
		<RoofMass>
			<EngineeringUnitsType>FmuMass</EngineeringUnitsType>
			<Value>0</Value>
		</RoofMass>
		<RoofType>FixedRoof</RoofType>
		<RoofLandingHeight>
			<EngineeringUnitsType>FmuLength</EngineeringUnitsType>
			<Value>0</Value>
		</RoofLandingHeight>
		<RoofFloatingHeight>
			<EngineeringUnitsType>FmuLength</EngineeringUnitsType>
			<Value>0</Value>
		</RoofFloatingHeight>
		<DatumHeight>
			<EngineeringUnitsType>FmuLength</EngineeringUnitsType>
			<Value>0</Value>
		</DatumHeight>
	</IndividualStrapTable>
  </StrapTables>
</StrapTable>' as [Value], 
'2015-02-04' as [CreatedDate],
'Administrator' as [CreatedBy],
'2015-02-04' as [UpdatedDate],
'Administrator' [UpdatedBy],
@StrapTablePointTemplatePropertyGuid as [PointTemplatePropertyGuid],
@PointTemplateGuid as [PointTemplateGuid]
) 
AS Source
ON (Target.[PointTemplatePropertyGuid] = Source.[PointTemplatePropertyGuid])
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
					target.[ValueType] = source.[ValueType],
					target.[Value] = source.[Value],
					target.[CreatedDate] = source.[CreatedDate],
					target.[CreatedBy] = source.[CreatedBy],
					target.[UpdatedDate] = source.[UpdatedDate],
					target.[UpdatedBy] = source.[UpdatedBy],
					target.[PointTemplatePropertyGuid] = source.[PointTemplatePropertyGuid],
					target.[PointTemplateGuid] = source.[PointTemplateGuid]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[ValueType],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplatePropertyGuid],[PointTemplateGuid])
	VALUES (source.[ID],source.[ValueType],source.[Value],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy],source.[PointTemplatePropertyGuid],source.[PointTemplateGuid]);

--Create Vessel Property
--Note : Vessel Value is xml serialization of FMBusinessObjects Vessel 
DECLARE @VesselPointTemplatePropertyGuid UNIQUEIDENTIFIER = '96FDC24A-2E74-4a3a-A20A-033659207A39'

MERGE dbo.tblPointTemplateProperty AS Target
USING 
(  SELECT 'Vessel' as [ID],
'FMBusinessObjects.DataObjects.Vessel' as [ValueType],
'<Vessel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <TankInstallationDate>2017-05-10T00:00:00-04:00</TankInstallationDate>
  <TankGeometry>VerticalCylinder</TankGeometry>
  <TankVolume>
	<EngineeringUnitsType>FmuVolume</EngineeringUnitsType>
	<Value>0</Value>
  </TankVolume>
  <TankHeight>
	<EngineeringUnitsType>FmuLength</EngineeringUnitsType>
	<Value>0</Value>
  </TankHeight>
  <TankRadius>
	<EngineeringUnitsType>FmuLength</EngineeringUnitsType>
	<Value>0</Value>
  </TankRadius>
  <TankShellThickness>
	<EngineeringUnitsType>FmuLength</EngineeringUnitsType>
	<Value>0</Value>
  </TankShellThickness>
  <TankLiningMaterial />
  <TankMaterial>MildCarbon</TankMaterial>
  <TankExpansionCoefficient>
	<EngineeringUnitsType>FmuNone</EngineeringUnitsType>
	<Value>1.24E-05</Value>
  </TankExpansionCoefficient>
  <TankInstallationTemperature>
	<EngineeringUnitsType>FmuTemp</EngineeringUnitsType>
	<Value>0</Value>
  </TankInstallationTemperature>
  <CathodicProtectionSupported>false</CathodicProtectionSupported>
  <OverfillProtectionSupported>false</OverfillProtectionSupported>
  <SpillProtectionSupported>false</SpillProtectionSupported>
  <TankShellCorrectionEnabled>false</TankShellCorrectionEnabled>
  <TankShellInsulated>false</TankShellInsulated>
  <AreaCoefficient>
	<EngineeringUnitsType>FmuNone</EngineeringUnitsType>
	<Value>4E-09</Value>
  </AreaCoefficient>
  <CSTManufacturerName />
  <CSTManufactureDate>2023-08-25T00:00:00-04:00</CSTManufactureDate>
  <CSTCapacity>
    <EngineeringUnitsType>FmuVolume</EngineeringUnitsType>
    <Value>0</Value>
  </CSTCapacity>
  <CSTSerialNumber />
  <CSTLocationName />
  <CSTLatitude xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" p2:nil="true" />
  <CSTLongitude xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" p2:nil="true" />
  <CSTCommissionDate>2023-08-25T00:00:00-04:00</CSTCommissionDate>
</Vessel>' as [Value], 
'2015-02-04' as [CreatedDate],
'Administrator' as [CreatedBy],
'2015-02-04' as [UpdatedDate],
'Administrator' [UpdatedBy],
@VesselPointTemplatePropertyGuid as [PointTemplatePropertyGuid],
@PointTemplateGuid as [PointTemplateGuid]
) 
AS Source
ON (Target.[PointTemplatePropertyGuid] = Source.[PointTemplatePropertyGuid])
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
					target.[ValueType] = source.[ValueType],
					target.[Value] = source.[Value],
					target.[CreatedDate] = source.[CreatedDate],
					target.[CreatedBy] = source.[CreatedBy],
					target.[UpdatedDate] = source.[UpdatedDate],
					target.[UpdatedBy] = source.[UpdatedBy],
					target.[PointTemplatePropertyGuid] = source.[PointTemplatePropertyGuid],
					target.[PointTemplateGuid] = source.[PointTemplateGuid]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[ValueType],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplatePropertyGuid],[PointTemplateGuid])
	VALUES (source.[ID],source.[ValueType],source.[Value],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy],source.[PointTemplatePropertyGuid],source.[PointTemplateGuid]);


--Create VcfSettings Property
--Note : VcfSettings Value is xml serialization of FMBusinessObjects VcfModuleSettings 
DECLARE @VcfSettingsPointTemplatePropertyGuid UNIQUEIDENTIFIER = '8529fc7f-7d00-4344-a968-58273c7ee6d7'

MERGE dbo.tblPointTemplateProperty AS Target
USING 
(  SELECT 'Volume Correction' as [ID],
'FMBusinessObjects.DataObjects.VcfModuleSettings' as [ValueType],
'<VcfModuleSettings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <DensityPressure>
	<EngineeringUnitsType>FmuPressure</EngineeringUnitsType>
	<Value>0</Value>
  </DensityPressure>
  <AlternateTemperature>
	<EngineeringUnitsType>FmuTemp</EngineeringUnitsType>
	<Value>0</Value>
  </AlternateTemperature>
  <BaseTemperature>
	<EngineeringUnitsType>FmuTemp</EngineeringUnitsType>
	<Value>60</Value>
  </BaseTemperature>
  <AlternateBasePressure>
	<EngineeringUnitsType>FmuPressure</EngineeringUnitsType>
	<Value>0</Value>
  </AlternateBasePressure>
  <K>
	<double>0</double>
	<double>0</double>
	<double>0</double>
	<double>0</double>
	<double>0</double>
  </K>
  <Alpha>0</Alpha>
  <UseProductObservedDensity>false</UseProductObservedDensity>
  <UseHydrometerCorrection>false</UseHydrometerCorrection>
  <ForceVcfTo4Digits>false</ForceVcfTo4Digits>
  <CorrectionMethodType>CORR_ASTM_COMM_2004</CorrectionMethodType>
  <CorrectionMethodSpecific>CORR_REFINED_PRODUCTS</CorrectionMethodSpecific>
</VcfModuleSettings>' as [Value], 
'2015-02-04' as [CreatedDate],
'Administrator' as [CreatedBy],
'2015-02-04' as [UpdatedDate],
'Administrator' [UpdatedBy],
@VcfSettingsPointTemplatePropertyGuid as [PointTemplatePropertyGuid],
@PointTemplateGuid as [PointTemplateGuid]
) 
AS Source
ON (Target.[PointTemplatePropertyGuid] = Source.[PointTemplatePropertyGuid])
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
					target.[ValueType] = source.[ValueType],
					target.[Value] = source.[Value],
					target.[CreatedDate] = source.[CreatedDate],
					target.[CreatedBy] = source.[CreatedBy],
					target.[UpdatedDate] = source.[UpdatedDate],
					target.[UpdatedBy] = source.[UpdatedBy],
					target.[PointTemplatePropertyGuid] = source.[PointTemplatePropertyGuid],
					target.[PointTemplateGuid] = source.[PointTemplateGuid]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[ValueType],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplatePropertyGuid],[PointTemplateGuid])
	VALUES (source.[ID],source.[ValueType],source.[Value],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy],source.[PointTemplatePropertyGuid],source.[PointTemplateGuid]);

--Create QuantitySettings Property
--Note : QuantitySettings Value is xml serialization of FMBusinessObjects QuantityModuleSettings 
DECLARE @QuantitySettingsPointTemplatePropertyGuid UNIQUEIDENTIFIER = 'E51C7A94-96CD-4E2B-AD1D-12E62F916998'

MERGE dbo.tblPointTemplateProperty AS Target
USING 
(  SELECT 'Quantity Settings' as [ID],
'FMBusinessObjects.DataObjects.QuantityModuleSettings' as [ValueType],
'<QuantityModuleSettings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <VolumeCalculationType>API2012Calculations</VolumeCalculationType>
  <MassOrWeightCalculationType>Mass</MassOrWeightCalculationType>
</QuantityModuleSettings>' as [Value], 
'2015-02-04' as [CreatedDate],
'Administrator' as [CreatedBy],
'2015-02-04' as [UpdatedDate],
'Administrator' [UpdatedBy],
@QuantitySettingsPointTemplatePropertyGuid as [PointTemplatePropertyGuid],
@PointTemplateGuid as [PointTemplateGuid]
) 
AS Source
ON (Target.[PointTemplatePropertyGuid] = Source.[PointTemplatePropertyGuid])
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
					target.[ValueType] = source.[ValueType],
					target.[Value] = source.[Value],
					target.[CreatedDate] = source.[CreatedDate],
					target.[CreatedBy] = source.[CreatedBy],
					target.[UpdatedDate] = source.[UpdatedDate],
					target.[UpdatedBy] = source.[UpdatedBy],
					target.[PointTemplatePropertyGuid] = source.[PointTemplatePropertyGuid],
					target.[PointTemplateGuid] = source.[PointTemplateGuid]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[ValueType],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplatePropertyGuid],[PointTemplateGuid])
	VALUES (source.[ID],source.[ValueType],source.[Value],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy],source.[PointTemplatePropertyGuid],source.[PointTemplateGuid]);

--Create Level RateSettings Property
--Note : RateSettings Value is xml serialization of FMBusinessObjects RateModuleSettings 
DECLARE @LevelRateSettingsPointTemplatePropertyGuid UNIQUEIDENTIFIER = '15443351-1677-43DE-8775-911D0885175B'

MERGE dbo.tblPointTemplateProperty AS Target
USING 
(  SELECT 'Rate Level Product Settings' as [ID],
'FMBusinessObjects.DataObjects.RateModuleSettings' as [ValueType],
'<RateModuleSettings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Deadband>0</Deadband>
  <StaleTimePeriodInSeconds>60</StaleTimePeriodInSeconds>
  <FlowCalculationType>Averaging</FlowCalculationType>
  <AveragingNumberSamples>4</AveragingNumberSamples>
  <AveragingSampleTimeSeconds>30</AveragingSampleTimeSeconds>
</RateModuleSettings>' as [Value], 
'2017-08-11' as [CreatedDate],
'Administrator' as [CreatedBy],
'2017-08-11' as [UpdatedDate],
'Administrator' [UpdatedBy],
@LevelRateSettingsPointTemplatePropertyGuid as [PointTemplatePropertyGuid],
@PointTemplateGuid as [PointTemplateGuid]
) 
AS Source
ON (Target.[PointTemplatePropertyGuid] = Source.[PointTemplatePropertyGuid])
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
					target.[ValueType] = source.[ValueType],
					target.[Value] = source.[Value],
					target.[CreatedDate] = source.[CreatedDate],
					target.[CreatedBy] = source.[CreatedBy],
					target.[UpdatedDate] = source.[UpdatedDate],
					target.[UpdatedBy] = source.[UpdatedBy],
					target.[PointTemplatePropertyGuid] = source.[PointTemplatePropertyGuid],
					target.[PointTemplateGuid] = source.[PointTemplateGuid]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[ValueType],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplatePropertyGuid],[PointTemplateGuid])
	VALUES (source.[ID],source.[ValueType],source.[Value],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy],source.[PointTemplatePropertyGuid],source.[PointTemplateGuid]);

--Create Total Observed RateSettings Property
--Note : RateSettings Value is xml serialization of FMBusinessObjects RateModuleSettings 
DECLARE @TotalObsRateSettingsPointTemplatePropertyGuid UNIQUEIDENTIFIER = '7FCD093E-3D52-46AA-8F4B-D68E21328423'

MERGE dbo.tblPointTemplateProperty AS Target
USING 
(  SELECT 'Rate Volume Total Observed Settings' as [ID],
'FMBusinessObjects.DataObjects.RateModuleSettings' as [ValueType],
'<RateModuleSettings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Deadband>0</Deadband>
  <StaleTimePeriodInSeconds>60</StaleTimePeriodInSeconds>
  <FlowCalculationType>Averaging</FlowCalculationType>
  <AveragingNumberSamples>4</AveragingNumberSamples>
  <AveragingSampleTimeSeconds>30</AveragingSampleTimeSeconds>
</RateModuleSettings>' as [Value], 
'2017-08-11' as [CreatedDate],
'Administrator' as [CreatedBy],
'2017-08-11' as [UpdatedDate],
'Administrator' [UpdatedBy],
@TotalObsRateSettingsPointTemplatePropertyGuid as [PointTemplatePropertyGuid],
@PointTemplateGuid as [PointTemplateGuid]
) 
AS Source
ON (Target.[PointTemplatePropertyGuid] = Source.[PointTemplatePropertyGuid])
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
					target.[ValueType] = source.[ValueType],
					target.[Value] = source.[Value],
					target.[CreatedDate] = source.[CreatedDate],
					target.[CreatedBy] = source.[CreatedBy],
					target.[UpdatedDate] = source.[UpdatedDate],
					target.[UpdatedBy] = source.[UpdatedBy],
					target.[PointTemplatePropertyGuid] = source.[PointTemplatePropertyGuid],
					target.[PointTemplateGuid] = source.[PointTemplateGuid]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[ValueType],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplatePropertyGuid],[PointTemplateGuid])
	VALUES (source.[ID],source.[ValueType],source.[Value],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy],source.[PointTemplatePropertyGuid],source.[PointTemplateGuid]);

--Create Net Standard (net) RateSettings Property
--Note : RateSettings Value is xml serialization of FMBusinessObjects RateModuleSettings 
DECLARE @NetRateSettingsPointTemplatePropertyGuid UNIQUEIDENTIFIER = '41EEBEF8-7020-47F2-A95A-3E084F6CE3DC'

MERGE dbo.tblPointTemplateProperty AS Target
USING 
(  SELECT 'Rate Volume Net Standard Settings' as [ID],
'FMBusinessObjects.DataObjects.RateModuleSettings' as [ValueType],
'<RateModuleSettings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Deadband>0</Deadband>
  <StaleTimePeriodInSeconds>60</StaleTimePeriodInSeconds>
  <FlowCalculationType>Averaging</FlowCalculationType>
  <AveragingNumberSamples>4</AveragingNumberSamples>
  <AveragingSampleTimeSeconds>30</AveragingSampleTimeSeconds>
</RateModuleSettings>' as [Value], 
'2017-08-11' as [CreatedDate],
'Administrator' as [CreatedBy],
'2017-08-11' as [UpdatedDate],
'Administrator' [UpdatedBy],
@NetRateSettingsPointTemplatePropertyGuid as [PointTemplatePropertyGuid],
@PointTemplateGuid as [PointTemplateGuid]
) 
AS Source
ON (Target.[PointTemplatePropertyGuid] = Source.[PointTemplatePropertyGuid])
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
					target.[ValueType] = source.[ValueType],
					target.[Value] = source.[Value],
					target.[CreatedDate] = source.[CreatedDate],
					target.[CreatedBy] = source.[CreatedBy],
					target.[UpdatedDate] = source.[UpdatedDate],
					target.[UpdatedBy] = source.[UpdatedBy],
					target.[PointTemplatePropertyGuid] = source.[PointTemplatePropertyGuid],
					target.[PointTemplateGuid] = source.[PointTemplateGuid]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[ValueType],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplatePropertyGuid],[PointTemplateGuid])
	VALUES (source.[ID],source.[ValueType],source.[Value],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy],source.[PointTemplatePropertyGuid],source.[PointTemplateGuid]);

--Create Gross Observed RateSettings Property
--Note : RateSettings Value is xml serialization of FMBusinessObjects RateModuleSettings 
DECLARE @GrossObsRateSettingsPointTemplatePropertyGuid UNIQUEIDENTIFIER = '90046419-3720-43BB-BDA6-BD74EE2A87E3'

MERGE dbo.tblPointTemplateProperty AS Target
USING 
(  SELECT 'Rate Volume Gross Observed Settings' as [ID],
'FMBusinessObjects.DataObjects.RateModuleSettings' as [ValueType],
'<RateModuleSettings xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Deadband>0</Deadband>
  <StaleTimePeriodInSeconds>60</StaleTimePeriodInSeconds>
  <FlowCalculationType>Averaging</FlowCalculationType>
  <AveragingNumberSamples>4</AveragingNumberSamples>
  <AveragingSampleTimeSeconds>30</AveragingSampleTimeSeconds>
</RateModuleSettings>' as [Value], 
'2017-08-11' as [CreatedDate],
'Administrator' as [CreatedBy],
'2017-08-11' as [UpdatedDate],
'Administrator' [UpdatedBy],
@GrossObsRateSettingsPointTemplatePropertyGuid as [PointTemplatePropertyGuid],
@PointTemplateGuid as [PointTemplateGuid]
) 
AS Source
ON (Target.[PointTemplatePropertyGuid] = Source.[PointTemplatePropertyGuid])
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
					target.[ValueType] = source.[ValueType],
					target.[Value] = source.[Value],
					target.[CreatedDate] = source.[CreatedDate],
					target.[CreatedBy] = source.[CreatedBy],
					target.[UpdatedDate] = source.[UpdatedDate],
					target.[UpdatedBy] = source.[UpdatedBy],
					target.[PointTemplatePropertyGuid] = source.[PointTemplatePropertyGuid],
					target.[PointTemplateGuid] = source.[PointTemplateGuid]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[ValueType],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplatePropertyGuid],[PointTemplateGuid])
	VALUES (source.[ID],source.[ValueType],source.[Value],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy],source.[PointTemplatePropertyGuid],source.[PointTemplateGuid]);


--Create TankCommandSettings Property
--Note : TankCommandSettings Value is xml serialization of FMBusinessObjects TankCommandModuleSettings 
DECLARE @TankCommandSettingsPointTemplatePropertyGuid UNIQUEIDENTIFIER = '5150A1F0-6E7B-480B-9AE2-F113669B1955'

MERGE dbo.tblPointTemplateProperty AS Target
USING 
(  SELECT 'Tank Command Settings' as [ID],
'FMBusinessObjects.DataObjects.TankCommandModuleSettings' as [ValueType],
'<TankCommandModuleSettings xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <MovementAlarmDifferential>
	<EngineeringUnitsType>FmuLength</EngineeringUnitsType>
	<Value>0</Value>
  </MovementAlarmDifferential>
</TankCommandModuleSettings>' as [Value],
'2015-02-04' as [CreatedDate],
'Administrator' as [CreatedBy],
'2015-02-04' as [UpdatedDate],
'Administrator' [UpdatedBy],
@TankCommandSettingsPointTemplatePropertyGuid as [PointTemplatePropertyGuid],
@PointTemplateGuid as [PointTemplateGuid]
) 
AS Source
ON (Target.[PointTemplatePropertyGuid] = Source.[PointTemplatePropertyGuid])
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
					target.[ValueType] = source.[ValueType],
					target.[Value] = source.[Value],
					target.[CreatedDate] = source.[CreatedDate],
					target.[CreatedBy] = source.[CreatedBy],
					target.[UpdatedDate] = source.[UpdatedDate],
					target.[UpdatedBy] = source.[UpdatedBy],
					target.[PointTemplatePropertyGuid] = source.[PointTemplatePropertyGuid],
					target.[PointTemplateGuid] = source.[PointTemplateGuid]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[ValueType],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplatePropertyGuid],[PointTemplateGuid])
	VALUES (source.[ID],source.[ValueType],source.[Value],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy],source.[PointTemplatePropertyGuid],source.[PointTemplateGuid]);


--Create TankTransferSettings Property
--Note : TankTransferSettings Value is xml serialization of FMBusinessObjects TankTransferModuleSettings 
DECLARE @TankTransferSettingsPointTemplatePropertyGuid UNIQUEIDENTIFIER = 'B2DA91E2-CD00-44FA-AB8D-DDA06A12B7BA'

MERGE dbo.tblPointTemplateProperty AS Target
USING 
(  SELECT 'Tank Transfer Settings' as [ID],
'FMBusinessObjects.DataObjects.TankTransferModuleSettings' as [ValueType],
'<TankTransferModuleSettings xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <TransferVolumeMode>GrossObservedVolume</TransferVolumeMode>
  <TransferAdvisoryTime>15</TransferAdvisoryTime>
  <CurrentTransferVolumeMode>GrossObservedVolume</CurrentTransferVolumeMode>
</TankTransferModuleSettings>' as [Value],
'2015-02-04' as [CreatedDate],
'Administrator' as [CreatedBy],
'2015-02-04' as [UpdatedDate],
'Administrator' [UpdatedBy],
@TankTransferSettingsPointTemplatePropertyGuid as [PointTemplatePropertyGuid],
@PointTemplateGuid as [PointTemplateGuid]
) 
AS Source
ON (Target.[PointTemplatePropertyGuid] = Source.[PointTemplatePropertyGuid])
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
					target.[ValueType] = source.[ValueType],
					target.[Value] = source.[Value],
					target.[CreatedDate] = source.[CreatedDate],
					target.[CreatedBy] = source.[CreatedBy],
					target.[UpdatedDate] = source.[UpdatedDate],
					target.[UpdatedBy] = source.[UpdatedBy],
					target.[PointTemplatePropertyGuid] = source.[PointTemplatePropertyGuid],
					target.[PointTemplateGuid] = source.[PointTemplateGuid]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[ValueType],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplatePropertyGuid],[PointTemplateGuid])
	VALUES (source.[ID],source.[ValueType],source.[Value],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy],source.[PointTemplatePropertyGuid],source.[PointTemplateGuid]);


--Create Leak Detection Settings Property
DECLARE @LeakDetectionSettingsPointTemplatePropertyGuid UNIQUEIDENTIFIER = '9043DA8B-C2D1-4744-976B-524DD600E036'

MERGE dbo.tblPointTemplateProperty AS Target
USING 
( SELECT 'Leak Detection Settings' AS [ID],
'FMBusinessObjects.DataObjects.LeakDetectionSettings' AS [ValueType],
'<LeakDetectionSettings>
	<AnalysisMethod>NetVolume</AnalysisMethod>
	<AnalysisType>Static</AnalysisType>
	<GaugeType>Generic</GaugeType>
	<AutoPrint>false</AutoPrint>
	<PrintDaysBeforeEOM>0</PrintDaysBeforeEOM>
	<PrintTime>0001-01-01T00:00:00-05:00</PrintTime>
	<MinimumFillPercentage>0</MinimumFillPercentage>
</LeakDetectionSettings>' AS [Value],
'2023-04-05' AS [CreatedDate],
'Administrator' AS [CreatedBy],
'2023-04-05' AS [UpdatedDate],
'Administrator' AS [UpdatedBy],
@LeakDetectionSettingsPointTemplatePropertyGuid AS [PointTemplatePropertyGuid],
@PointTemplateGuid AS [PointTemplateGuid]
) 
AS Source
ON (Target.[PointTemplatePropertyGuid] = Source.[PointTemplatePropertyGuid])
WHEN MATCHED THEN
	UPDATE SET target.[ID] = source.[ID],
					target.[ValueType] = source.[ValueType],
					target.[Value] = source.[Value],
					target.[CreatedDate] = source.[CreatedDate],
					target.[CreatedBy] = source.[CreatedBy],
					target.[UpdatedDate] = source.[UpdatedDate],
					target.[UpdatedBy] = source.[UpdatedBy],
					target.[PointTemplatePropertyGuid] = source.[PointTemplatePropertyGuid],
					target.[PointTemplateGuid] = source.[PointTemplateGuid]
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([ID],[ValueType],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplatePropertyGuid],[PointTemplateGuid])
	VALUES (source.[ID],source.[ValueType],source.[Value],source.[CreatedDate],source.[CreatedBy],source.[UpdatedDate],source.[UpdatedBy],source.[PointTemplatePropertyGuid],source.[PointTemplateGuid]);


--Entity Assignment
if(0 = (SELECT COUNT(*) FROM map.tblEntityPointTemplateToSite WHERE PointTemplateGuid = @PointTemplateGuid AND SiteGuid = @TankSiteGuid))
BEGIN
		INSERT INTO map.tblEntityPointTemplateToSite ([PointTemplateToSiteGuid],[PointTemplateGuid],[SiteGuid],[AssignedFromSiteGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy]) VALUES ('CB824B93-A908-4AED-8F50-226ACD83DBEE',@PointTemplateGuid,@TankSiteGuid,@TankSiteGuid,'2015-02-04','Administrator','2015-02-04','Administrator')
END 

