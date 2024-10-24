import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
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
export class MessageComponent implements OnInit, AfterViewInit{
  
  private user: UserData | null = null;
  private isUpdate: boolean = false;
  private editingMessageId = '';
  private page: number = 1;
  private pageSize: number = 20;
  private isLoading: boolean = false;
  private hasMoreMessages: boolean = true;

  public messages: Message[] = [];
  public searchQuery: string = '';
  public hoveredMessage: number | null = null;
  public isEmojiPickerVisible: boolean = false;
  public newMessage: string = '';
  public newFile: File | null = null;
  public newFileName: string | null = null;
  public attachedFile: MessageFile | null = null;

  @ViewChild('messagesBox') messagesBox!: ElementRef;
  @ViewChild('fileInput') fileInput!: ElementRef;

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

  ngAfterViewInit(): void {
    this.scrollToBottom();
  }

  public onScroll(event: any) {
    const { scrollTop, scrollHeight, clientHeight } = event.target;
    const isTop = scrollHeight + scrollTop === clientHeight + 1;
    if (isTop && !this.isLoading && this.hasMoreMessages) {
      this.page++;
      this.getMessages(false);
    }
  }

  public getMessagesWithQuery(){
    this.resetMessageForm();
    this.getMessages(true);
  }

  public deleteMessage(messageId: string){
    this.messageService.deleteMessage(messageId).subscribe((res: any)=>{
      this.resetMessageForm();
      this.getMessages();
    });
  }

  public updateMessage(message: Message){
    this.newMessage = message.text;
    this.isUpdate = true;
    this.editingMessageId = message.id;
    this.attachedFile = message.messageFile ? message.messageFile : null;
    this.newFile = null;
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
      this.newFileName = this.newFile.name;
      this.attachedFile = null;
    } else {
      this.newFile = null;
      this.newFileName = null;
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

  public isWithinEditableTime(message: any): boolean {
    const createdTime = new Date(message.createdDate).getTime();
    const currentTime = new Date().getTime();
    const fifteenMinutesInMs = 15 * 60 * 1000;

    return (currentTime - createdTime) <= fifteenMinutesInMs;
  }

  private getMessages(isScrollToBottom: boolean = true): void {
    this.isLoading = true;

    this.messageService.getMessages(this.user!.id, this.searchQuery, this.page, this.pageSize).subscribe((res: any) => {
      const newMessageFromApi = res.map((message: any) => new Message(message));
      this.messages = [...this.messages, ...newMessageFromApi];
      this.isLoading = false;
      this.hasMoreMessages = newMessageFromApi.length === this.pageSize;

      if (isScrollToBottom) {
        this.scrollToBottom();
      }
    });
  }

  private scrollToBottom(): void {
    setTimeout(() => {
      if (this.messagesBox) {
        this.messagesBox.nativeElement.scrollTop = this.messagesBox.nativeElement.scrollHeight;
      }
    }, 0);
  }

  private CreateMessageForSend(){
    var newMsg = {
      UserId: this.user?.id,
      Text: this.newMessage,
      File: this.newFile
    };
    this.messageService.addNewMessage(newMsg).subscribe((res: any)=>{
      this.resetMessageForm();
      this.getMessages(true);
    });
  }

  private CreateUpdateMessageForSend(){
    const updateMsg: any = {
      MessageId: this.editingMessageId,
      Text: this.newMessage
    };

    if (this.attachedFile instanceof MessageFile) {
      updateMsg.File = {
        fileData: this.attachedFile.fileData,
        fileName: this.attachedFile.fileName
      };
    } else if (this.newFile) {
      updateMsg.File = this.newFile;
    }
    this.messageService.updateMessage(updateMsg).subscribe((res: any)=>{
      this.resetMessageForm();
      this.getMessages(true);
    });
  }

  private resetMessageForm(){
    this.newMessage = '';
    this.isUpdate = false;
    this.editingMessageId = '';
    this.newFile = null;
    this.fileInput.nativeElement.value = '';
    this.newFileName = null;
    this.page = 1;
    this.messages = [];
  }
}
