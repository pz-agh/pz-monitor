var timelineWidget;
var resources;
var measurement_times;
    
$(document).ready(function() {    
    resources=new Array();
    
    $( "#progressbar" ).progressbar();
    $( "#progressbar" ).progressbar("value", 0);
    
    var url='http://localhost:8081/resources/';
    console.log('Pobieranie danych o resourceach');
    getJsonData(url,'r', 0);
})

function setAllStuff(data, type, whichResource){
    if(data==null){
        alert('Błąd pobierania danych!');
        $( "#progressbar" ).hide();
    }            
    else{
        if(type=='r'){
            $( "#progressbar" ).progressbar("value", 25);
            var resource = new Resource();
            parseResources(data, resource);
            resources.push(resource);
            getMeasurementsIds();
        }
        else if(type=='i'){
            $( "#progressbar" ).progressbar("value", 50);
            parseMeasurementIds(data, whichResource);
            getMeasurements();
        }
        else if(type=='m'){
            $( "#progressbar" ).progressbar("value", 75);
            parseMeasurement(data, whichResource);
            if(resources[whichResource].measurements.length == resources[whichResource].measurements_ids.length){
                generateContent();
                $( "#progressbar" ).progressbar("value", 100);
                setTimeout(function(){$( "#progressbar" ).hide()}, 1000);
            }
        }
    }
}

function getJsonpData(url, type, whichResource){
    $.ajax({
        url: url,
        dataType: 'jsonp',
        crossDomain: true,
        xhrFields: {
            withCredentials: true
        },
        username:"test@liferay.com",
        password:"test",
        success: function(data){
            console.log("success");
            console.log(data);
            
            var jsonData = JSON.parse(data);
            
            setAllStuff(jsonData, type, whichResource)
        }
    })
    .fail(function(jqxhr, textStatus, error) {
        var err = textStatus + ", " + error;
        console.log( "Request Failed: " + err );
        $( "#progressbar" ).hide();
    })
}

function getJsonData(url, type, whichResource){
    jQuery.getJSON('php/Dane.php?callback=?', {
        addr:url,
        format: 'json'
    }, function(data) {
        console.log( "success" );
        console.log(data);
        
        setAllStuff(data, type, whichResource)
    })
    .fail(function(jqxhr, textStatus, error) {
        var err = textStatus + ", " + error;
        console.log( "Request Failed: " + err );
        $( "#progressbar" ).hide();
    });
}

function generateContent(){
    console.log( 'Udalo sie' );
    addRowsToTable();
} 

function parseMeasurement(data, whichResource){
    var resource = resources[whichResource];
    resource.setName(data.resource_name);
    resource.setCpuCores(data.cpu_cores);
    resource.setTotalMemory(data.total_memory);
    resource.setMeasurement(data.measurement_time, data.free_memory, data.cpu_usage, data.measurement_id);
}

function getMeasurements(){
    console.log('Pobieranie danych o pomiarach');
    for(var i=0; i<resources.length; i++){
        for(var j=0; j<resources[i].measurements_ids.length; j++){
            var measurement_id = resources[i].measurements_ids[j];
            var url='http://localhost:8080/api/secure/json?serviceClassName=monitor.service.MeasurementServiceUtil&serviceMethodName=getMeasurement&measurementId='+measurement_id+'&serviceParameters=[measurementId]';
            getJsonpData(url, 'm', i);
            console.log('i co teraz2');
        }
    }
}

function getMeasurementsIds(){
    console.log('Pobieranie danych o measurement id');
    for(var i=0; i<resources.length; i++){
        var resource_id = resources[i].id;
        var url='http://localhost:8080/api/secure/json?serviceClassName=monitor.service.MeasurementServiceUtil&serviceMethodName=getMeasurements&resourceId='+resource_id+'&serviceParameters=[resourceId]';
        getJsonpData(url, 'i', i);
        console.log('i co teraz');
    }
}

function parseMeasurementIds(data, whichResource){
    for(var i=0; i<data.length; i++){
        var measurement_id = data[i].measurement_id;
        resources[whichResource].addMeasurementId(measurement_id);
    }
}

function parseResources(data, resource){
    for(var i=0; i<data.length; i++){
        var resource_id = data[i].resource_id;
        resource.setId(resource_id);
    }
}

function addRowsToTable(){
    var table = document.getElementById("resourcesTable");
    
    for(var i=0; i<resources.length; i++){
        var resource = resources[i];
        
        var row = table.insertRow(i+1);
        var idCell = row.insertCell(0);
        var nameCell = row.insertCell(1);
        var countCell = row.insertCell(2);
        var cpuCoresCell = row.insertCell(3);
        var totalMemoryCell = row.insertCell(4);
        
        idCell.innerHTML = resource.id;
        nameCell.innerHTML = resource.name;
        countCell.innerHTML = resource.getCountOfMeasurements();
        cpuCoresCell.innerHTML = resource.cpu_cores;
        totalMemoryCell.innerHTML = resource.total_memory;
    }
}

function goToChart(type){
    console.log('i jest '+type);
                
    var options = {
        chart: {
            renderTo: 'container',
            type: 'spline'
        },
        series: [{}]
    };

    timelineWidget = new TimelineWidget(type);  
}