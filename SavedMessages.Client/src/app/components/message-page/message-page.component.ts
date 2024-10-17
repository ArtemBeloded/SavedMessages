import { CommonModule } from '@angular/common';
import { AfterViewChecked, AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user-service/user.service';
import { Router } from '@angular/router';
import { MessageService } from '../../services/message-service/message.service';
import { UserData } from '../../models/user-model';
import { Message } from '../../models/messages.model';
import { PickerComponent } from '@ctrl/ngx-emoji-mart';
import { MessageFile } from '../../models/message-file.model';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';

@Component({
  selector: 'app-message-page',
  standalone: true,
  imports: [FormsModule, CommonModule, PickerComponent, InfiniteScrollModule ],
  templateUrl: './message-page.component.html',
  styleUrl: './message-page.component.scss'
})
export class MessageComponent implements OnInit, AfterViewChecked{
  
  private user: UserData | null = null;
  private isUpdate: boolean = false;
  private editingMessageId = '';
  private isLoading: boolean = false;
  private page: number = 1;
  private pageSize: number = 20;
  private hasMoreMessages: boolean = true; // Флаг для остановки подгрузки

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

//   ngAfterViewInit(): void {
//     this.messageElements.changes.subscribe(() => {
//       setTimeout(() => {
//         if (this.messagesContainer && this.messagesContainer.nativeElement) {
//           console.log('Messages container and native element exist');
//           this.setupObserver();
//         } else {
//           console.error('Messages container or native element not available in ngAfterViewInit');
//         }
//       }, 0);
//     });
// }
  
//   @ViewChild('messagesContainer') private messagesContainer!: ElementRef;
//   @ViewChildren('messageElement') messageElements!: QueryList<ElementRef>; 
  
//   private setupObserver(): void {
//     console.log('from setupObserver');

//     if (!this.messagesContainer || !this.messagesContainer.nativeElement) {
//         console.error('messagesContainer or nativeElement is not available');
//         return;
//     }

//     const observer = new IntersectionObserver((entries) => {
//         console.log('Entries observed:', entries);
//         entries.forEach(entry => {
//             console.log('Observed element isIntersecting:', entry.isIntersecting);
//             if (entry.isIntersecting && !this.isLoading && this.hasMoreMessages) {
//                 this.getMessages();
//             }
//         });
//     }, { root: this.messagesContainer.nativeElement, threshold: 0.5 });

//     console.log('Message elements:', this.messageElements);

//     this.messageElements.changes.subscribe(() => {
//         if (this.messageElements.length > 2) {
//             const thirdFromLastIndex = this.messageElements.length - 3;

//             if (thirdFromLastIndex >= 0) {
//                 const thirdFromLastMessage = this.messageElements.toArray()[thirdFromLastIndex];
//                 if (thirdFromLastMessage) {
//                     console.log('Third from last message:', thirdFromLastMessage);
//                     observer.observe(thirdFromLastMessage.nativeElement);
//                 }
//             }
//         }
//     });

//     // Наблюдение за элементом при первом вызове
//     if (this.messageElements.length > 2) {
//         const thirdFromLastIndex = this.messageElements.length - 3;
//         const thirdFromLastMessage = this.messageElements.toArray()[thirdFromLastIndex];
//         if (thirdFromLastMessage) {
//             observer.observe(thirdFromLastMessage.nativeElement);
//         }
//     }
// }


// public getMessages(): void {
//   if (this.isLoading || !this.hasMoreMessages) return;

//   this.isLoading = true;
//     // Сохраняем текущее положение скролла
//     const currentScrollTop = this.messagesContainer.nativeElement.scrollTop;


//   this.messageService.getMessages(this.user!.id, this.searchQuery, this.page, this.pageSize).subscribe((res: any) => {
//     const newMessages = res.map((message: any) => new Message(message));

//     if (newMessages.length === 0) {
//       this.hasMoreMessages = false;
//     } else {
//       this.messages = [...this.messages, ...newMessages]; // Сообщения добавляются в начало списка
//       this.page++;
//     }

//     this.isLoading = false;
//     this.messagesContainer.nativeElement.scrollTop = currentScrollTop + (this.messagesContainer.nativeElement.scrollHeight - this.messagesContainer.nativeElement.clientHeight);
//   });
// }

public onScroll(event: any) {
  const offset = event.target.scrollTop + event.target.clientHeight;
  const height = event.target.scrollHeight;
  if (offset >= height - 100) {
    console.log('on scroll activ')
  }
}

  public getMessages(){
    this.isLoading = true;
    this.messageService.getMessages(this.user!.id, this.searchQuery, this.page, this.pageSize).subscribe((res: any) =>{
      const newMessageFromApi = res.map((message: any)=> new Message(message));
      this.messages = [...this.messages, ...newMessageFromApi];
      this.isLoading = false;
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


  // public onScroll(event: any){
  //   const conteiner = event.target;
  //   const scrollPosition = conteiner.scrollTop;

  //   if(scrollPosition === 0 && !this.isLoading){
  //     this.page++;
  //     this.getMessages();
  //     console.log('from onScroll IF')
  //   }
  // }

  

  

 

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
