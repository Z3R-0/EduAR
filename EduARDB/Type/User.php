<?php
namespace eduar\Type;

use eduar\Connect;

class User
{
	private $conn;

	public function __construct(Connect $connect)
	{
		$this->conn = $connect->getConnection();
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
		if($this->conn->exec($query) === false) {
			return "Error occured while creating User";
		} else {
			return "User succesfully created";
		}
	}

	public function update($query, $table)
	{
		$result = $this->conn->exec($query);
		if($result === false) {
			return "Error occured while updating User";
		} else {
			return $result;
		}
	}

	public function delete($query)
	{
		if($this->conn->exec($query) === false) {
			return "Error occured while trying to remove User";
		} else {
			return "User succesfully removed";
		}
	}

}