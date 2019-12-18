<?php

namespace eduar;

use eduar\Type\User;
use eduar\Type\Scenario;

CONST METHODS = ['get', 'create', 'update', 'delete', 'createOrUpdate'];

class RequestHandler
{
	private $conn;

	public function __construct(Connect $connect)
	{
		$this->conn = $connect->getConnection();
	}

	public function execute($type, $method, $query)
	{
		try {
			if (in_array($method, METHODS)) {
				return $this->$method($query, $type);
			} else {
				$type = ucfirst($type);
				$class = new $type($this->conn);
				return $class->$method($query);
			}
		} catch (\Exception $e) {
			return $e;
		}
	}

	public function encryptQuery($string, $method, $key, $iv)
	{
		return base64_encode(openssl_encrypt($string, $method, $key, OPENSSL_RAW_DATA, $iv));
	}

	public function decryptQuery($encrypted, $method, $key, $iv)
	{
		return openssl_decrypt(base64_decode($encrypted), $method, $key, OPENSSL_RAW_DATA, $iv);
	}

	public function get($query, $type = null)
	{
		$pdoQuery = $this->conn->query($query);

		if($type == "ScenarioAI") {
			$result = $pdoQuery->fetch();
			return $result['AUTO_INCREMENT'];
		} else {
			$result = $pdoQuery->fetchAll(\PDO::FETCH_ASSOC);
			$arr = [];
			if (count($result) > 0) {
				foreach ($result as $row) {
					$arr[] = $row;
				}
				return $arr;
			} else {
				return '';
			}
		}
	}

	public function create($query, $type = null)
	{
		if ($this->conn->exec($query) === false) {
			return "Error occurred while trying to execute query";
		} else {
			return $this->conn->exec("SELECT * FROM ");
		}
	}

	public function update($query, $options = null)
	{
		if(preg_match('/password = \'(.*?)\'/', $query, $match) !== false) {
			$pass = hash('sha256', $match[1]);
			$query = preg_replace('/password = \''. $match[1].'\'/', 'password = \''. $pass.'\'', $query);
		}
		$result = $this->conn->exec($query);

		if ($result === false) {
			return "Error occurred while trying to execute query";
		} else {
			if(!empty($options)) {
				$type = $options['type'];
				$token = $options['token'];
				if ($type == 'pass_reset') {
					$this->conn->exec("UPDATE teacher SET password_reset_token = NULL, password_reset_expiration = NULL WHERE password_reset_token = '$token';");
				}
			}
			return $result;
		}
	}

	public function createOrUpdate($query, $type = null)
	{
		return $this->execTransaction($query);
	}

	public function delete($query, $type = null)
	{
		return $this->execTransaction($query);
	}

	private function execTransaction($query) {
		$queries = explode(";", $query);

		if(count($queries) > 1 && !empty($queries[1])) {
			try {
				$this->conn->beginTransaction();
				foreach ($queries as $query) {
					$this->conn->query($query . ";");
				}
				$this->conn->commit();
				return $this->conn->lastInsertId();
			} catch(\Exception $e) {
				$this->conn->rollBack();
				return $e->getMessage() . "\r\n" . $e->getTraceAsString();
			}
		} else {
			if($this->conn->exec($queries[0]) === false) {
				return "Error occurred while trying to execute query";
			} else {
				return $this->conn->lastInsertId();
			}
		}
	}
}