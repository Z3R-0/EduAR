<?php
namespace eduar;

class Connect
{
	private function getConnection()
	{
		$host = 'localhost';
		$user = 'root';
		$pass = '';
		$db = 'eduar';

		try
		{
			return new \PDO('mysql:host=' . $host . ';dbname=' . $db, $user, $pass);
		}

		catch
		(\Exception $e) {
			echo 'Err occured mfucker';
			die();
		}
	}

	public function getQueryResult($query)
	{
		$conn = $this->getConnection();
		$pdoQuery = $conn->query($query);

		$result = $pdoQuery->fetchAll(\PDO::FETCH_ASSOC);

		if (count($result) > 0) {
			foreach ($result as $row) {
				echo $row;
			}
		}
	}
}
