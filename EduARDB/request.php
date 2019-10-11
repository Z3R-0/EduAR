<?php
namespace eduar;

require_once 'autoload.php';

$connect = new Connect();
$requestHandler = new RequestHandler($connect);

$type = $_GET['type'];
$method = $_GET['method'];
$query = $_GET['query'];

echo json_encode($requestHandler->execute($type, $method, $query));