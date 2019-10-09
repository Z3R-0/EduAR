<?php
namespace eduar;

require_once('Connect.php');

$connector = new Connect();

$param = $_GET['query'];

echo $connector->getQueryResult($param);