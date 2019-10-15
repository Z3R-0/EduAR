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

	public function get($query)
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

	public function create($query)
	{
		if ($this->conn->exec($query) === false) {
			return "Error occurred while creating User";
		} else {
			return "User successfully created";
		}
	}

	public function update($query, $table)
	{
		$result = $this->conn->exec($query);
		if ($result === false) {
			return "Error occurred while updating User";
		} else {
			return $result;
		}
	}

	public function delete($query)
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