<?php
namespace eduar;

require_once 'autoload.php';

$requestHandler = new RequestHandler();

$type = $_GET['type'];
$method = $_GET['method'];
$query = $_GET['query'];

echo json_encode($requestHandler->execute($type, $method, $query));