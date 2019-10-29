<?php
namespace eduar;

require_once 'autoload.php';

$connect = new Connect();
$requestHandler = new RequestHandler($connect);

if($_SERVER['REQUEST_METHOD'] === 'POST') {
	echo json_encode($requestHandler->update($_POST['query'], $_POST['options']));
} else {
	$type = $_GET['type'];
	$method = $_GET['method'];
	$query = $_GET['query'];
	echo json_encode($requestHandler->execute($type, $method, $query));
}