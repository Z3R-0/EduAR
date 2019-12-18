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
    <link rel="stylesheet" type="text/css"
          href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css"/>
    <link rel="stylesheet" type="text/css" href="https://eduarapp.000webhostapp.com/style/style.css"/>
</head>
<body>
<section class="col-sm-6 content-wrap">
    <form id="createForm" class="contact-form" action="" method="post">
        <span id="handleError" style="display: none;"></span>
        <div class="col-sm-12">
            <div class="input-block">
                <label for="email">Email</label>
                <input id="email" class="form-control" type="email" required/>
            </div>
        </div>
        <div class="col-sm-12">
            <div class="input-block">
                <label for="password">Wachtwoord</label>
                <input id="password" class="form-control" type="password" required minlength="8"/>
            </div>
        </div>
        <div class="col-sm-12">
            <button id="submitForm" class="square-button" type="submit">Opslaan</button>
        </div>
    </form>
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
            let query = "INSERT INTO teacher () VALUES();";
            $.ajax({
                url: '/request.php',
                method: 'POST',
                dataType: 'json',
                data: {query: query},
                success: function (response) {
                    $('#submitForm').attr('disabled', true);
                    $('#handleError').html("Password successfully updated! <br/> This page can now be closed. <br/>");
                    $('#handleError').show();
                },
                error: function (response) {
                    console.log(response);
                }
            });
        }
    })
</script>
