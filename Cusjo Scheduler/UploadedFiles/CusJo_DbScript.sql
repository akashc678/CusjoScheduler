

create database CusJo
use cusjo
create table UserList
(
Id int Identity(1,1),
Name varchar(500),
EmailId varchar(500),
IsEmailLinkOpen bit default(0),
ReminderCnt int default(0)
)

--drop table UserList

insert into UserList (Name,EmailId) values ('Akash','akashchoudhary678@gmail.com')
insert into UserList (Name,EmailId) values ('Piyush','piyushchaudhary2008@gmail.com')
insert into UserList (Name,EmailId) values ('Palak','piyushchaudhary9029@gmail.com')








create database CusJo
use cusjo
create table UserList
(
Id int Identity(1,1),
Name varchar(500),
EmailId varchar(500),
IsEmailLinkOpen bit default(0),
ReminderCnt int default(0)
)

--drop table UserList

insert into UserList (Name,EmailId) values ('Akash','akashchoudhary678@gmail.com')
insert into UserList (Name,EmailId) values ('Piyush','piyushchaudhary2008@gmail.com')
insert into UserList (Name,EmailId) values ('Palak','piyushchaudhary9029@gmail.com')
insert into UserList (Name,EmailId) values ('Akash','akashchoudhary1011@gmail.com')


select * from UserList(nolock)

update UserList set isemaillinkopen=0,ReminderCnt=2


alter procedure sp_CusJoEmail
(
@Id int=null,
@ReminderCnt int=null,
@mode varchar(500)
)
as
Begin
	if @mode='GetEmailList'
		Begin
			select id, EmailId from UserList where ReminderCnt < 3 and IsEmailLinkOpen=0
		End

	if @mode='UpdEmailReminderCnt'
		Begin
		   update UserList set ReminderCnt=ReminderCnt+1 where Id=@Id
		End

	if @mode='UpdIsEmailLinkOpen'
		Begin
		   update UserList set IsEmailLinkOpen=1 where Id=@Id
		End
End