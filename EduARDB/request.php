<?php
namespace eduar;

require_once 'autoload.php';

$connect = new Connect();
$requestHandler = new RequestHandler($connect);

if(isset($_GET['key'])) {
	$key = substr(hash('sha256', $connect->getSecretSalt(), true), 0, 32);
	$iv = chr(0x1) . chr(0x4) . chr(0x7) . chr(0x2) . chr(0x5) . chr(0x8) . chr(0x3) . chr(0x6) . chr(0x9) . chr(0x4) . chr(0x7) . chr(0x10) . chr(0x5) . chr(0x8) . chr(0x11) . chr(0x6);

	$query = explode('&key', $_SERVER['QUERY_STRING'], 2);

	$encrypted = $requestHandler->encryptQuery(urldecode($query[0]), 'aes-256-cbc', $key, $iv);

	if($encrypted !== $_GET['key']) {
		echo "Unauthorized request";
		die();
	}

	if ($_SERVER['REQUEST_METHOD'] === 'POST') {
		echo json_encode($requestHandler->update($_POST['query'], $_POST['options']));
	} else {
		$type = $_GET['type'];
		$method = $_GET['method'];
		$query = $_GET['query'];
		echo json_encode($requestHandler->execute($type, $method, $query));
	}
} else {
	echo "Missing encryption key";
}