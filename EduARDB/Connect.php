<?php
namespace eduar;

class Connect
{
	private $host;
	private $user;
	private $pass;
	private $db;
	private $conn;

	public function __construct()
	{
		$this->getDbCredentials();
		$this->conn = $this->initConnection();
	}

	private function getDbCredentials()
	{
		//TODO Get credentials from settings file?
		$this->host = 'localhost';
		$this->user = 'root';
		$this->pass = '';
		$this->db = 'eduar';
	}

	private function initConnection()
	{
		try
		{
			return new \PDO('mysql:host=' . $this->host . ';dbname=' . $this->db, $this->user, $this->pass);
		}

		catch
		(\Exception $e) {
			echo 'Err occured mfucker';
			die();
		}
	}

	public function getConnection()
	{
		return $this->conn;
	}

	public function getQueryResult($query)
	{
		$pdoQuery = $this->conn->query($query);

		$result = $pdoQuery->fetchAll(\PDO::FETCH_ASSOC);

		if (count($result) > 0) {
			foreach ($result as $row) {
				echo json_encode($row);
			}
		}
	}
}
