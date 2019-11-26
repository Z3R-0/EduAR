<?php
require_once 'autoload.php';

use eduar\Connect;
use eduar\RequestHandler;

$requestHandler = new RequestHandler(new Connect());

ini_set('display_errors', 1);
?>
<!DOCTYPE html>
<html>
<head>
    <title>EduAR Password Reset</title>
    <link rel="stylesheet" type="text/css" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css"/>
    <link rel="stylesheet" type="text/css" href="https://eduarapp.000webhostapp.com/style/style.css"/>
</head>
<body>
<section class="col-sm-6 content-wrap">
	<?php if (!empty($_GET)) { ?>
		<?php if (isset($_GET['type']) && $_GET['type'] == 'mail') {
			try {
				$token = bin2hex(random_bytes(16));
				echo $token . "\r\n";
				$expiration_date = date("Y-m-d h:i:s", strtotime('+1 day'));
				$user = $requestHandler->get("SELECT * FROM teacher WHERE email = '" . $_GET['email'] . "';");
				if ($user) {
					echo "user exists \r\n";
					$requestHandler->update("
			    UPDATE teacher 
			    SET password_reset_token = '$token', password_reset_expiration = '$expiration_date' 
			    WHERE email = '" . $_GET['email'] . "';
			    ");

					$url = "https://eduarapp.000webhostapp.com/passwordreset.php?token=" . $token;
					$template = file_get_contents("passwordreset_email.html");
					$template = str_replace("{{ url }}", $url, $template);

					$to = $_GET['email'];
					$subject = "EduAR Password Reset Requested";
					$message = $template;
					$headers = "MIME-Version: 1.0" . "\r\n";
					$headers .= "Content-type:text/html;charset=UTF-8" . "\r\n";
					$headers .= 'From: <support@eduar.com>' . "\r\n";

					mail($to, $subject, $message, $headers);
					echo "Email has been sent \r\n";
				}
			} catch (\Exception $e) {

			}
		} else { ?>
			<?php if (isset($_GET['token'])) { ?>
				<?php
				$token = $_GET['token'];
				$user = $requestHandler->get("SELECT * FROM teacher WHERE password_reset_token = '$token'");
				if (!empty($user)) {
					$curr_date = strtotime('now');
					$expiration_date = strtotime($user[0]['password_reset_expiration']);
					if ($expiration_date > $curr_date) { ?>
              <form id="resetForm" class="contact-form" action="" method="post">
                  <span id="handleError" style="display: none;"></span>
                  <input id="token" hidden value="<?php echo $token ?>"/>
                  <div class="col-sm-12">
                      <div class="input-block">
                          <label for="newPass">Nieuw Wachtwoord</label>
                          <input id="newPass" class="form-control" type="password" required/>
                      </div>
                  </div>
                  <div class="col-sm-12">
                      <div class="input-block">
                          <label for="confirmPass">Bevestig wachtwoord</label>
                          <input id="confirmPass" class="form-control" type="password" required/>
                      </div>
                  </div>
                  <div class="col-sm-12">
                      <button id="submitForm" class="square-button" type="submit">Opslaan</button>
                  </div>
              </form>
					<?php } else {
						echo "<p>Requested token is expired.</p>";
					}
				} else {
					echo "<p>Requested token does not exist.</p>";
				}
			} else {
				echo "<p></p>";
			} ?>
		<?php }
	} else { ?>
      <h1>Missing password reset token.</h1>
	<?php } ?>
</section>
</body>
<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
<script src="js/main.js"></script>
</html>

<script type="text/javascript">
    $('#submitForm').on('click', function (e) {
        e.preventDefault();
        if ($('#newPass').val() !== $('#confirmPass').val()) {
            $('#handleError').css('display', 'block');
            $('#handleError').html('Entered passwords do not match.');
            $('#confirmPass').val("");
        } else {
            $('#handleError').val("").css('display', 'none');
            let query = "update teacher set password = '" + $('#confirmPass').val() + "' where password_reset_token = '" + $('#token').val() + "';";
            $.ajax({
                url: '/request.php',
                method: 'POST',
                dataType: 'json',
                data: {query: query, options: {type: 'pass_reset', token: $('#token').val()}},
                success: function (response) {
                    $('#submitForm').attr('disabled', true);
                    $('#handleError').html("Password successfully updated! <br/> This page can now be closed. <br/>");
                    $('#handleError').show();
                },
                error: function(response) {
                    console.log(response);
                }
            });
        }
    })
</script>