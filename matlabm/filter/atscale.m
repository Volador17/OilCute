% ��׼����������У�������������У��������Ʒ���μ�atscalep��
% input
% x:����Ⱦ���
% 
% output
% xc:����������Ⱦ���
% mx:ƽ�����ף�
% sx:��׼ƫ����ף�

function [xc,mx,sx] = atscale(x);
	[m,n]=size(x);
	mx=mean(x,2);
	sx=std(x,[],2);
	II=find(sx==0);%Ѱ��sxΪ0����,�ڼ���xcʱ������㣬����Ϊ0
	xc=zeros(m,n);
	III=1:m;
	III(II)=[];
	xc(III,:)=(x(III,:)-mx(III,:)*ones(1,n))./(sx(III,:)*ones(1,n));
end