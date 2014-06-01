<!DOCTYPE html>
<html>
    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
        <script src="//ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" type="text/javascript"></script>
        <link rel="stylesheet" href="//code.jquery.com/ui/1.10.4/themes/smoothness/jquery-ui.css">
        <script src="//code.jquery.com/jquery-1.10.2.js"></script>
        <script src="//code.jquery.com/ui/1.10.4/jquery-ui.js"></script>
        <script src="http://code.highcharts.com/stock/highstock.js"></script>
        <script src="http://code.highcharts.com/stock/modules/exporting.js"></script>
        <script type="text/javascript" src="js/myHighstock.js"></script>
        <script type="text/javascript" src="js/functions.js"></script>
        <script type="text/javascript" src="js/resource.js"></script>
        <script type="text/javascript" src="js/measurement.js"></script>
        <link rel="stylesheet" href="css/style.css">
    </head>
    <body>
        <div id="resources" style="overflow: hidden">
            <table id="resourcesTable" style="width:500px; float:left;">
                <tr>
                    <td>id zasobu</td>
                    <td>nazwa zasobu</td> 
                    <td>ilosc pomiarow</td>
                    <td>liczba rdzeni</td>
                    <td>cakowita pamiec</td>
                </tr>
            </table>
            <div id="buttons" style="float:left;">
                <input type="button" value="free memory" onclick='goToChart("free_memory")'/>
                <input type="button" value="cpu usage" onclick='goToChart("cpu_usage")' />
                <input type="button" value="Wykonej nowy pomiar" onClick="window.location.reload()">
            </div><br/>
        </div>
        <div id="progressbar"><div class="progress-label">Loading...</div></div>
        <div id="container" style="height: 500px; min-width: 500px">
        </div>​
        <div id="error" style="height: 500px; min-width: 500px"></div>​
    </body>
</html>
