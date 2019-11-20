<?php
namespace eduar;

require_once 'autoload.php';

$connect = new Connect();
$requestHandler = new RequestHandler($connect);

if(isset($_GET['key']) || isset($_POST['key'])) {
	$key = substr(hash('sha256', $connect->getSecretSalt(), true), 0, 32);
	$iv = chr(0x1) . chr(0x4) . chr(0x7) . chr(0x2) . chr(0x5) . chr(0x8) . chr(0x3) . chr(0x6) . chr(0x9) . chr(0x4) . chr(0x7) . chr(0x10) . chr(0x5) . chr(0x8) . chr(0x11) . chr(0x6);

	if(isset($_GET['key'])) {
		$query = explode('&key', $_SERVER['QUERY_STRING'], 2);
	} else {
		$query[0] = $_POST['query'];
	}

	$encrypted = $requestHandler->encryptQuery(urldecode($query[0]), 'aes-256-cbc', $key, $iv);

	if(isset($_GET['key'])) {
		$requestKey = $_GET['key'];
	} else {
		$requestKey = $_POST['key'];
	}

	if($encrypted !== str_replace(" ", "+", $requestKey)) {
		echo "Unauthorized request";
		die();
	}

	if ($_SERVER['REQUEST_METHOD'] === 'POST') {
		if($_POST['method'] == 'createOrUpdate') {
			echo json_encode($requestHandler->execute($_POST['type'], $_POST['method'], $_POST['query']));
		} else {
			echo json_encode($requestHandler->update($_POST['query'], $_POST['options']));
		}
	} else {
		$type = $_GET['type'];
		$method = $_GET['method'];
		$query = $_GET['query'];
		echo json_encode($requestHandler->execute($type, $method, $query));
	}
} else {
	echo "Missing encryption key";
}