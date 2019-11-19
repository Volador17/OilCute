function [TQ,SQ] = mwcorrVector(x,y,wind,topk)
% �ƶ����ϵ������ȷ���ƶ����ϵͳ��ƽ��ֵ��ֵ
% ���룺x�������ظ��Թ���1��y�������ظ��Թ���2��wind�����ڿ�ȣ�
% �����TQ�������ƶ����ϵͳ��ƽ��ֵ��ֵ��SQ���������ƶ����ϵ����ֵ��

	[m,n]=size(x);% m�����ױ�����Ŀ��
	SQ=zeros(1,m);
	for k=1:m-wind
		xx=x(k:k+wind,:)';
		yy=y(k:k+wind,:)';
		SQ(k)=corre(xx,yy);
	end
	or=ones(1,wind)*corre(xx,yy);
	SQ(m-wind+1:end)=or;
	SQ=abs(SQ);
    [m1,n1]=sort(SQ);
    SQ(n1(1:topk-1)) = [];
    %SQ = SQ(n1(topk:end));
    
	oo=SQ(1:m-wind-topk+1);
	TQ = sum(oo)/(m-wind-topk+1);% ������ֵ
	%TQ = sum(oo.*oo)/(m-wind);% ������ֵ
end
