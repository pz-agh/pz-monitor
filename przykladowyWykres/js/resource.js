function Resource(){
    this.id;
    this.name;
    this.measurements_ids=new Array();
    this.measurements = new Array();
    this.cpu_cores;
    this.total_memory;
    
    this.setId = function setId(newId){
        this.id=newId;
    }
    
    this.addMeasurementId = function addMeasurementId(mId){
        this.measurements_ids.push(mId);
    }
    
    this.getCountOfMeasurements = function getCountOfMeasurements(){
        return this.measurements_ids.length;
    }
    
    this.setMeasurement = function setMeasurement(time, freeMemory, cpuUsage, id){
        var measurement = new Measurement();
        
        measurement = {
            measurement_time:time,
            free_memory:freeMemory,
            cpu_usage:cpuUsage,
            measurements_id:id
        };
        this.measurements.push(measurement);
    }
    
    this.setName = function setName(resName){
        this.name=resName;
    }
    
    this.setCpuCores = function setCpuCores(coutOfCores){
        this.cpu_cores = coutOfCores;
    }
    
    this.setTotalMemory = function setTotalMemory(newTotalMemory){
        this.total_memory = newTotalMemory;
    }
}