import { CommonModule } from '@angular/common';
import { AfterViewChecked, AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user-service/user.service';
import { Router } from '@angular/router';
import { MessageService } from '../../services/message-service/message.service';
import { UserData } from '../../models/user-model';
import { Message } from '../../models/messages.model';
import { PickerComponent } from '@ctrl/ngx-emoji-mart';
import { MessageFile } from '../../models/message-file.model';

@Component({
  selector: 'app-message-page',
  standalone: true,
  imports: [FormsModule, CommonModule, PickerComponent],
  templateUrl: './message-page.component.html',
  styleUrl: './message-page.component.scss'
})
export class MessageComponent implements OnInit, AfterViewChecked{
  
  private user: UserData | null = null;
  private isUpdate: boolean = false;
  private editingMessageId = '';
  public messages: Message[] = [];
  public searchQuery: string = '';
  public hoveredMessage: number | null = null;
  public isEmojiPickerVisible: boolean = false;
  public newMessage: string = '';
  public newFile: File | null = null;
  public newFileName: string | null = null;
  public attachedFile: MessageFile | null = null;

  constructor(
    private userService: UserService,
    private router: Router,
    private messageService: MessageService
  ){}

  ngOnInit(): void {
    if(!this.userService.isAuthenticate()){
      this.router.navigateByUrl('Login');
    }
    else{
      this.user = this.userService.getUserData();
      this.getMessages();
    }
  }

  public getMessages(){
    this.messageService.getMessages(this.user!.id, this.searchQuery).subscribe((res: any) =>{
      this.messages = res.map((message: any)=> new Message(message));
    });
  }

  public deleteMessage(messageId: string){
    this.messageService.deleteMessage(messageId).subscribe((res: any)=>{
      this.getMessages();
    });
  }

  public updateMessage(message: Message){
    this.newMessage = message.text;
    this.isUpdate = true;
    this.editingMessageId = message.id;
    this.attachedFile = message.messageFile ? message.messageFile : null;
    this.newFile = null; // Сбрасываем новый файл, если он был
  }

  public toggleEmojiPicker() {
    this.isEmojiPickerVisible = !this.isEmojiPickerVisible;
  }

  public addEmoji(event: any) {
    this.newMessage += event.emoji.native;
    this.toggleEmojiPicker();
  }

  public attachFile(event: any) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.newFile = input.files[0];
      this.newFileName = this.newFile.name; // Show the selected file's name
      this.attachedFile = null; // Скрываем старый файл
    } else {
      this.newFile = null;
      this.newFileName = null; // Clear the file name if no file is selected
    }
  }

  public removeAttachedFile(){
    this.attachedFile = null;
  }

  public sendMessage(){
    if(this.newMessage.trim() || 
      this.newFile instanceof File ||
      this.attachedFile instanceof MessageFile){
      if(this.isUpdate){
        this.CreateUpdateMessageForSend();
      }
      else{
        this.CreateMessageForSend();
      }
    }
  }

  private CreateMessageForSend(){
    var newMsg = {
      UserId: this.user?.id,
      Text: this.newMessage,
      File: this.newFile
    };
    this.messageService.addNewMessage(newMsg).subscribe((res: any)=>{
      this.getMessages();
      this.resetMessageForm();
    });
  }

  private CreateUpdateMessageForSend(){
      // Создаем базовый объект для отправки сообщения
  const updateMsg: any = {
    MessageId: this.editingMessageId,
    Text: this.newMessage
  };

  // Проверяем, что файл прикреплен, и добавляем его в объект
  if (this.attachedFile instanceof MessageFile) {
    updateMsg.File = {
      fileData: this.attachedFile.fileData,
      fileName: this.attachedFile.fileName
    };
  } else if (this.newFile) {
    updateMsg.File = this.newFile;
  }
    this.messageService.updateMessage(updateMsg).subscribe((res: any)=>{
      this.getMessages();
      this.resetMessageForm();
    });
  }

  @ViewChild('fileInput') fileInput!: ElementRef;
  private resetMessageForm(){
    this.newMessage = '';
    this.isUpdate = false;
    this.editingMessageId = '';
    this.newFile = null;
    this.fileInput.nativeElement.value = ''; // Очистить input file
    this.newFileName = null;
  }

  

  

  //@ViewChild('messagesContainer') private messagesContainer!: ElementRef;

  // ngAfterViewInit() {
  //   console.log('from ngAfterViewInit')
  //   this.scrollToBottom();
  // }

  // public scrollToBottom(): void {
  //   if (this.messagesContainer?.nativeElement) {
  //       console.log(this.messagesContainer?.nativeElement)
  //       this.messagesContainer.nativeElement.scrollTop = this.messagesContainer.nativeElement.scrollHeight;
  //   }
  // }

  ngAfterViewChecked() {
    // с помощью этого можно попробывать подгружать елементы в массив
  }
}
