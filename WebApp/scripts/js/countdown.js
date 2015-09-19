function setTimeCount(container, str)
{
	$('#'+container).each(function(){
	
		$(this).html(str);
	});
}

function countdown(container, Time_Left)
{
   var saveTime = Time_Left;
	if (Time_Left > 0)
	{
		days = Math.floor(Time_Left / (60 * 60 * 24));
		Time_Left %= (60 * 60 * 24);
		hours = Math.floor(Time_Left / (60 * 60));
		Time_Left %= (60 * 60);
		minutes = Math.floor(Time_Left / 60);
		seconds = Time_Left % 60;
		
		var str = '';
		var flag = false;
		if (days > 0)
		{
			str += ((days<10)?('0'+days):days) + ' <sup>ngày</sup> ';
			flag = true;
		}
		if (flag || (hours > 0))
		{
			str += ((hours<10)?('0'+hours):hours)  + ' <sup>giờ</sup> ';
			flag = true;
		}
		if (flag || (minutes > 0))
		{
			str += ((minutes<10)?('0'+minutes):minutes) + ' <sup>phút</sup> ';
			flag = true;
		}
		if (flag || (seconds > 0))
		{
			str += ((seconds<10)?('0'+seconds):seconds)+' <sup>giây</sup>';
		}
		setTimeCount(container, str);
		setTimeout('countdown("' + container + '",' + (saveTime-1) + ');', 1000);

	 }
	 else
	 {
		setTimeCount(container, 'Kết Thúc');
		
	 }
	 
}