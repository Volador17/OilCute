function [net,Loads,Scores,Weights,b,yreg,ye,md,mdt,my,R,sec,rms,centerSpecData,Score_length,nnd,nndt]=plsann1(x,y,pp,trainf,hm,f1,f2,tn,traino,kk)

%�޼�ؼ���ƫ��С���ˣ��˹������罨ģ����
%net�������������
%Loads:�غɣ�
%Scores:�÷֣�
%Weights:Ȩ��ʸ����
%b:�ع�ϵ����
%yreg:Ԥ��ֵ��
%ye���в
%md:���Ͼ��룻
%mdt�����Ͼ�����ֵ��
%my��y��ƽ��ֵ��
%R:Ԥ��ֵ��ʵ��ֵ֮������ϵ����
%sec��У����Ԥ���׼ƫ�
%rms�����ײв

%x�����׾���
%y������������
%pp������������
%trainf:ѵ��������ȡֵΪtraingd��traingdm��trainbfg��trainlm�е�һ��
%hm�������ڵ�����
%f1:��һ�㴫�ݺ�����ȡֵΪtansig,logsig,purelin�е�һ����
%f2:�ڶ��㴫�ݺ�����ȡֵΪtansig,logsig,purelin�е�һ����
%tn:ѵ��������
%traino:ѵ��Ŀ��.
% kk: �ⲿѭ������

y= y';

[m,n]=size(x);
x=x';
centerSpecData = mean(x);
calx = x - ones(size(x,1),1) * centerSpecData;
calx=calx';
my=mean(y);
caly=y-my;%y�ľ�ֵ���Ļ���
[Scores,Loads,Weights,b,Score_length] = pls1(calx,caly',pp);%ƫ��С���ˣ��õ��غɺ͵÷֡�
%�������Ͼ����Լ����Ͼ�����ֵ
md=diag(Scores*Scores');
mdt=3*pp/m;
%������ײв�
X =calx-Loads*Scores';  
rms=(sum((X).^2)).^0.5;
%�������ڽ�������ֵ
nnd=nndr(Scores',Scores');
nndt=max(nnd);
%-------------------------------------------------------------------------
for i=1:kk

net=bann1(Scores',caly,trainf,hm,f1,f2,tn,traino);%�����������

yy1=sim(net,Scores')+my*ones(1,n);%����Ԥ��ֵ��
yy2(i,:)=yy1;
end
yreg=mean(yy2);
ye=yreg-y;%����в

R=(sum((yreg-my).*(yreg-my)))/(sum(ye.*ye)+sum((yreg-my).*(yreg-my)));%�������ϵ����

sec=sqrt(ye*ye'/(n-1));%����Ԥ���׼ƫ��



