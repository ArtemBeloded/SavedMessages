import { HttpClient, HttpParams } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../../enviroment/enviroment";
import { Message } from "../../models/messages.model";

@Injectable({
    providedIn: 'root'
})
export class MessageService{
    private httpClient = inject(HttpClient);
    private apiURL: string;

    constructor(){
        this.apiURL = environment.apiURL;
    }

    public getMessages(userId: string, searchQuery: string, page: number, pageSize: number){
        let params = new HttpParams();
        params = params.append('userId', userId);
        params = params.append('searchTerm', searchQuery);
        params = params.append('page', page);
        params = params.append('pageSize', pageSize);
        return this.httpClient.get<Message[]>(this.apiURL + '/api/messages', {params: params});
    }

    public addNewMessage(newMessage: any){
        const formData = new FormData();
        formData.append('UserId', newMessage.UserId);
        formData.append('Text', newMessage.Text);
        formData.append('File', newMessage.File);
        return this.httpClient.post(this.apiURL + '/api/messages/addmessage', formData);
    }

    public deleteMessage(messageId: string){
        return this.httpClient.delete(this.apiURL + '/api/messages/' + messageId);
    }

    public updateMessage(messageForUpdate: any){
        const formData = new FormData();
        formData.append('Text', messageForUpdate.Text);
        if (messageForUpdate.File instanceof File) {
            // Новый файл
            formData.append('File', messageForUpdate.File);
        } else if (messageForUpdate.File && messageForUpdate.File.fileData) {
            // Старый файл, если он не был заменен
            formData.append('File', new Blob([messageForUpdate.File.fileData]), messageForUpdate.File.fileName);
        }
        return this.httpClient.put(this.apiURL + '/api/messages/' + messageForUpdate.MessageId, formData);
    }
}