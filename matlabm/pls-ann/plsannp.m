function [y,md,rms,nnd, ss]=plsannp(x,net,Weights,Loads,Scores,b,my,centerSpecData,Score_length);
    
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
[m,n]=size(x);
x=x';
valx = x - ones(size(x,1),1) * centerSpecData;
valx=valx';
[ss] =plssc2(valx,Loads,Weights,b,Score_length);;%������֤���÷֡�
md=diag(ss*ss');;
%������ײв�
X =valx-Loads*ss';  
rms=(sum((X).^2)).^0.5;
nnd=nndr(ss',Scores');
y=sim(net,ss');
y = y +my*ones(1,n);%����Ԥ�����ʣ�
