% һ�ײ��
% input
% X:����Ⱦ���
% m:��ֿ��
% output
% xc:�����Ĺ���

function d=deriate1(X,m)
	[m1,n]=size(X);
	for i=1:m1-m
		d(i,:)=X(i+m,:)-X(i,:);
	end
	d((m1-m+1):m1,:)=zeros(m,n);
end
