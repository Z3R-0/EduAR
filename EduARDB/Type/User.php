<?php
namespace eduar;

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

		if (count($result) > 0) {
			foreach ($result as $row) {
				return $row;
			}
		} else {
			return '';
		}
	}

	public function create($query)
	{

	}

	public function update($query)
	{

	}

	public function delete($query)
	{

	}

}