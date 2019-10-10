<?php
namespace eduar;

use eduar\Type\User;
use eduar\Type\Scenario;

class RequestHandler
{
	private $conn;

	public function __construct()
	{
		$this->conn = new Connect();
	}

	public function execute($type, $method, $query)
	{
		try {
			switch ($type) {
				case 'User':
					$user = new User($this->conn);
					return $user->$method($query);
					break;

				case 'Scenario' :
					$scenario = new Scenario($this->conn);
					return $scenario->$method($query);
					break;

				default :
					throw new \Exception("Type " . $type . " not found.");
			}
		} catch(\Exception $e) {
			return $e;
		}
	}
}