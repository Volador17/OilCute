% ��׼����������У��������Ʒ���������У�������μ�atscale��
% input
% x:����Ⱦ���
% mx:ƽ�����ף�
% sx:��׼ƫ����ף�
% output
% xc:����������Ⱦ���

function [xc]=atscalep(x,mx,sx);
	[m,n]=size(x);
	II=find(sx==0);%Ѱ��sxΪ0����,�ڼ���xcʱ������㣬����Ϊ0
	xc=zeros(m,n);
	III=1:m;
	III(II)=[];
	xc(III,:)=(x(III,:)-mx(III,:)*ones(1,n))./(sx(III,:)*ones(1,n));
end