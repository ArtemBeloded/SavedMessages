import { MessageFile } from "./message-file.model"

export class Message{
    id: string = '';
    createdDate: Date = new Date;
    text: string = '';
    isEdited: boolean = false;
    messageFile: MessageFile | null = null;

    constructor(data: any) {
        this.id = data.id;
        this.createdDate = new Date(data.createdInUtc);
        this.text = data.text;
        this.isEdited = data.isEdited;

        
        if(data.file.fileName){
          this.messageFile = new MessageFile;
          this.messageFile.fileName = data.file.fileName;
          this.messageFile.fileData = this.ConvertBase64ToBlob(data.file.fileData, data.file.contentType);
          this.messageFile.url = URL.createObjectURL(this.messageFile.fileData);
        }
      }

      private ConvertBase64ToBlob(base64Data: string, contentType: string): Blob {
        const byteCharacters = atob(base64Data);
        const byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
          byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        return new Blob([byteArray], { type: contentType });
      }
}