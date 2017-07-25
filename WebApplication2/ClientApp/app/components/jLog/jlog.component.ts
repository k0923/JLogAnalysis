import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'jlog',
    templateUrl: './jlog.component.html',
    styleUrls: ['./jlog.component.css']
})
export class JLogComponent {
    model: JLogModel;

    private http: Http;

    private url: string;

    type: Array<string>;

    constructor(http: Http, @Inject('ORIGIN_URL') originUrl: string) {
        this.http = http;
        this.url = originUrl;
        this.model = { LogData: "", IsJson: false, Result: '' };
        this.type = ["1", "3"];
    }

    post() {
        
        
        this.http.post(this.url + '/api/jlog', this.model).subscribe(result => {
          
            this.model.IsJson = result.json().isJson;
            this.model.LogData = result.json().logData;
            this.model.Result = result.json().result;
            


            
        });

       
    }


}

interface JLogModel {
    LogData: string;
    IsJson: boolean;
    Result: string;
    
}

