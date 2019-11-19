%��Ԫɢ��У������Multiplicative Scatter Correction��MSC�������ڴ���У�������ף�����У��������׵ĳ����mscp��
% input
% x:����Ⱦ���
% output
% xc:����������Ⱦ���
% mx:У������ƽ�����ף�

% reference : Applied Spectroscopy, Volumn 39,Number 4, 1985, 641-646
function[xc,mx]=msc(x);
	x=x';
	[m,n]=size(x);
	mx=mean(x);
	x2=mean(x');
	x3=mean(mx);
	b(m)=zeros;
	a(m)=zeros;
	xc(m,n)=zeros;
    for i=1:m;
		b(i)=(sum((x(i,:)-x2(i)).*(mx-x3)))/sum((mx-x3).*(mx-x3));
		a(i)=x2(i)-b(i)*x3;
		xc(i,:)=(x(i,:)-a(i))/b(i);
    end
	xc=xc';mx=mx';
end






 