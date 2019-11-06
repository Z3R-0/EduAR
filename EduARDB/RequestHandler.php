<?php

namespace eduar;

use eduar\Type\User;
use eduar\Type\Scenario;

CONST METHODS = ['get', 'create', 'update', 'delete'];

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
				return $this->$method($query);
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

	public function get($query, $options = null)
	{
		$pdoQuery = $this->conn->query($query);
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

	public function create($query, $options = null)
	{
		if ($this->conn->exec($query) === false) {
			return "Error occurred while creating User";
		} else {
			return "User successfully created";
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
			return "Error occurred while updating User";
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

	public function delete($query, $options = null)
	{
		if ($this->conn->exec($query) === false) {
			return "Error occurred while trying to remove User";
		} else {
			return "User successfully removed";
		}
	}

	//				switch ($type) {
//					case 'User':
//						$user = new User($this->conn);
//						return $user->$method($query);
//						break;
//
//					case 'Scenario' :
//						$scenario = new Scenario($this->conn);
//						return $scenario->$method($query);
//						break;
//
//					default :
//						throw new \Exception("Type " . $type . " not found.");
//				}
}