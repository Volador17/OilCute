function [y,SEP,vR,ye,md,rms,nnd]=plsannv(x,vy,net,Weights,Loads,b,my,centerSpecData,Score_length);
    
%ƫ��С�����˹�������Ԥ�����
%x�����׾���
%net:�������
%Loads:�غɣ�
%Weights:Ȩ��ʸ����
%b:�ع�ϵ����
%my��y��ƽ��ֵ��
%y��Ԥ�����ʣ�
%md:���Ͼ��룻
%rms:���ײв
%Score_length   �÷������ĳ���  
% У�����������Ļ�ֵ
 mmy=mean(vy);   
[m,n]=size(x);
x=x';
calx = x - ones(size(x,1),1) * centerSpecData;
calx=calx';
[Scores] =plssc2(calx,Loads,Weights,b,Score_length);;%������֤���÷֡�
md=diag(Scores*Scores');;
%������ײв�
X =calx-Loads*Scores';  
rms=(sum((X).^2)).^0.5;
nnd=nndr(Scores',Scores');
y=sim(net,Scores')+my*ones(1,n);%����Ԥ�����ʣ�

ye=y-vy;%����в
vR=(sum((y-mmy).*(y-mmy)))/(sum(ye.*ye)+sum((y-mmy).*(y-mmy)));%�������ϵ����
SEP=sqrt(ye*ye'/(n-1));%����Ԥ���׼ƫ��
