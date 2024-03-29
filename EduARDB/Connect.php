<?php
namespace eduar;

class Connect
{
	private $host;
	private $user;
	private $pass;
	private $db;
	private $conn;
	private $ini;

	public function __construct()
	{
		$this->ini = parse_ini_file('settings.ini');

		$this->getDbCredentials();
		$this->conn = $this->initConnection();
	}

	public function getSecretSalt()
	{
		return $this->ini['secret_salt'];
	}

	private function getDbCredentials()
	{
		$this->host = $this->ini['db_host'];
		$this->user = $this->ini['db_user'];
		$this->pass = $this->ini['db_pass'];
		$this->db = $this->ini['db_name'];
	}

	private function initConnection()
	{
		try
		{
			return new \PDO('mysql:host=' . $this->host . ';dbname=' . $this->db, $this->user, $this->pass);
		}

		catch
		(\Exception $e) {
			echo $e->getMessage() . "\r\n" . $e->getTraceAsString();
			die();
		}
	}

	public function getConnection()
	{
		return $this->conn;
	}
}
