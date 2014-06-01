<?php

header('Content-type: application/json;');
header('Access-Control-Allow-Origin: *');

shell_exec("java -jar D:/lf/pz-monitor/sensor.jar");

$addr = $_GET["addr"];
$cb = $_GET["callback"];

$username = "test@liferay.com";
$password = "test";

set_time_limit(3600);
sleep(3);

$str_data = file_get_contents($addr);

$data = json_decode($str_data, true);

echo $cb . "(" . json_encode($data) . ")";
?>