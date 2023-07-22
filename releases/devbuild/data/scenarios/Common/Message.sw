// Global message controls

float _commonMessage_standard_duration = 5;
// internal variables
float _commonMessage_short_duration = 3;
float _commonMessage_flashdanger_time = 3;
float _commonMessage_flashdanger_freq = 0.15;
float3 _commonMessage_color_yellow = { 0.8, 0.8, 0 };
float3 _commonMessage_color_orange = { 1, 0.5, 0 };
float3 _commonMessage_color_red = { 0.8, 0, 0 };


Common.MessageStandard(string message, float3 color):
	Message(message, _commonMessage_standard_duration, color, 1);


Common.MessageUnlogged(string message, float3 color):
	Message(message, _commonMessage_standard_duration, color, 1, false);
    
   
Common.MessageShort(string message, float3 color):
	Message(message, _commonMessage_short_duration, color, 1);


Common.MessageShortUnlogged(string message, float3 color):
	Message(message, _commonMessage_short_duration, color, 1, false);


Common.MessageFlashDanger(string message):
	Common.MessageShort(message, _commonMessage_color_orange);
	float time = 0;
	while (time < _commonMessage_flashdanger_time)
	{
		AddEvent(time, "Common.MessageShortUnlogged", message, _commonMessage_color_yellow);
		AddEvent(time + _commonMessage_flashdanger_freq, "Common.MessageShortUnlogged", message, _commonMessage_color_orange);
		time += _commonMessage_flashdanger_freq * 2;
	}

Common.MessageImportant(string message, float3 color):
    // Use priority 50
	Message(message, _commonMessage_standard_duration, color, 50);


Common.MessageCritical(string message, float3 color):
    // Use priority 99
	Message(message, _commonMessage_standard_duration, color, 99);