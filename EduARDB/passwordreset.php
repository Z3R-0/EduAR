<!DOCTYPE html>
<html>
<head>
	<title></title>
	<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
</head>
<body>
<div>
	<?php
	if (!empty($_GET)): ?>
		<form id="resetForm" action="" method="get">
			<span id="handleError" style="display: none;"></span>
            <input id="email" hidden value="<?php echo $_GET['email'] ?>"/>
			<input id="newPass" type="password" placeholder="New password" required/>
			<input id="confirmPass" type="password" placeholder="Confirm password" required/>
			<input type="submit" value="Change Password" />
		</form>
	<?php else : ?>
		<p>Reset token not recognized or expired; </p>
	<?php endif; ?>
</div>
</body>
</html>

<script type="text/javascript">
	$('#resetForm').on('submit', function(e) {
	    e.preventDefault();
			if($('#newPass').val() !== $('#confirmPass').val()) {
			    $('#handleError').css('display', 'block');
			    $('#handleError').html('Entered passwords do not match.');
			    $('#confirmPass').val("");
			} else {
			    $('#handleError').val("").css('display', 'none');
			    let query = "update teacher set password = '" + $('#confirmPass').val() + "' where email = '" + $('#email').val() + "';";
			    $.ajax({
                    url: 'http://localhost/eduar/request.php?type=user&method=update&query=' + query,
                    method: 'GET',
                    success: function(response) {
                        console.log(response);
                    }
                });
			}
	})
</script>

